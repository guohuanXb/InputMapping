using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native.UIKit.Framework
{
    public class UIManager :AbstractSystem,IUIManager
    {
        private const string RootLocation = "UIRoot";
        private IResourceSystem _resourceSystem;
        /// <summary>
        /// 所有面板元数据
        /// </summary>
        private Dictionary<Type,PanelData> _panelDic;
        private Dictionary<UILayer,Transform> _uiLayerDic;
        //TODO：释放
        /// <summary>
        /// 切换场景时应该被释放掉
        /// </summary>
        private AssetHandle _rootHandle;
        protected override void OnInit()
        {
            _resourceSystem = new YooAssetResourceSystem();
            _panelDic = new();
            LoadAllPanelData().Forget();
        }

        private async UniTaskVoid LoadAllPanelData()
        {
            var packageName = this.GetModel<IPackageModel>().DefaultPackageName;
            var jsonHandle =await _resourceSystem.LoadAssetAsync<TextAsset>("PanelConfigs", packageName);
            var jsonAsset = jsonHandle.AssetObject as TextAsset;
            if (jsonAsset == null)
            {
                Debug.LogError($"PanelConfigs is Empty");
                return;
            }
            
            //TODO:Fix
            var panelSerializationData = JsonConvert.DeserializeObject<List<PanelSerializationData>>(jsonAsset.text);
            
            var asmDict = AppDomain.CurrentDomain.GetAssemblies()
                .ToDictionary(a => a.GetName().Name);
            
            foreach (var data in panelSerializationData)
            {
                Type type = null;

                if (asmDict.TryGetValue(data.AssemblyName, out var asm))
                {
                    type = asm.GetType(data.ClassName);
                }

                if (type == null || !typeof(IPanel).IsAssignableFrom(type))
                {
                    Debug.LogError($"找不到 Panel 类型: {data.ClassName} (程序集: {data.AssemblyName})");
                    continue;
                }
                var panelData = new PanelData(state: PanelState.UnLoad, config: data.Configs);
                _panelDic.Add(type,panelData);
            }
        }
        

        public async UniTask InstantiateLayer()
        {
            var packageModel = this.GetModel<IPackageModel>();
            _rootHandle = await _resourceSystem.LoadAssetAsync<GameObject>(RootLocation,packageModel.DefaultPackageName);
            var rootPrefab = _rootHandle.AssetObject as GameObject;
            var root =  GameObject.Instantiate(rootPrefab);
            foreach (Transform child in root.transform)
            {
                var layer = Enum.Parse<UILayer>(child.name,ignoreCase:true);
                _uiLayerDic.Add(layer,child);
            }
        }

        public async UniTask LoadPanelAsync<T>() where T : IPanel
        {
            var panelType = typeof(T);
            if(!_panelDic.TryGetValue(panelType,out var panelData))
                return;
            var config = panelData.Config;
            var handle = await _resourceSystem.LoadAssetAsync<GameObject>(config.Location,config.PackageName);
            _panelDic[panelType].State = PanelState.Loaded;
            _panelDic[panelType].Handle = handle;
        }
        
        public T GetPanel<T>()where T : IPanel
        {
            if(_panelDic.TryGetValue(typeof(T),out var panelData) &&
               panelData.IsOpened &&
               panelData.Panel !=null)
                return (T)panelData.Panel;
            return default;
        }

        public T OpenPanel<T>()where T : IPanel
        {
            var panelType = typeof(T);
            if (!_panelDic.TryGetValue(panelType, out var panelData))
            {
                Debug.LogError($"{panelType} is not Exist");
                return default;
            }

            if (panelData.State == PanelState.UnLoad)
            {
                Debug.LogError($"{panelType} is not Loaded");
                return default;
            }

            if (panelData.State == PanelState.Loaded)
            {
                var panelPrefab = panelData.Handle.AssetObject as GameObject;
                if (panelPrefab == null || !panelPrefab.TryGetComponent<T>(out var prefabPanel))
                {
                    Debug.LogError($"{panelType} Component is not Loaded to Prefab");
                    return default;
                }
                var layerRoot = _uiLayerDic[prefabPanel.Layer];
                var instance = GameObject.Instantiate(panelPrefab, layerRoot);
                var panel = instance.GetComponent<T>();  
                panelData.State = PanelState.Instantiated;
                panelData.Panel = panel;
                panel.OnOpen();
                panelData.IsOpened = true;
                return panel;
            }

            if (panelData.State == PanelState.Instantiated)
            {
                var panel = panelData.Panel;
                panel.OnOpen();
                panelData.IsOpened = true;
                return (T)panel;
            }

            return default;
        }

        public async UniTask<T> OpenPanelAsync<T>()where T : IPanel
        {
            await LoadPanelAsync<T>();
            return OpenPanel<T>();
        }

        public void FocusPanel(IPanel panel)
        {
            panel.OnFocus();
        }


        public void ClosePanel<T>()where T : IPanel
        {
            if (_panelDic.TryGetValue(typeof(T), out var panelData) &&
                panelData.State == PanelState.Instantiated &&
                panelData.IsOpened)
            {
                panelData.Panel.OnClose();
                panelData.IsOpened = false;
            }
        }

        public void RemovePanelRef<T>() where T : IPanel
        {
            if (_panelDic.TryGetValue(typeof(T), out var panelData))
            {
                panelData.Panel = null;
                panelData.State = PanelState.Loaded;
            }
        }

        public void UnLoadPanel<T>() where T : IPanel
        {
            if (_panelDic.TryGetValue(typeof(T), out var panelData))
            {
                panelData.Panel = null;
                panelData.Handle?.Release();
                panelData.Handle = null;
                panelData.State = PanelState.UnLoad;
            }
        }
    }
}