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
        /// 切换场景时应该被释放掉
        /// </summary>
        private Dictionary<Type,AssetHandle> _panelAssetDic;
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
            _panelAssetDic = new();
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
                var panelData = new PanelData(state: PanelState.UnLoad, config: data.Configs, isFocus: false);
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
            _panelAssetDic.Add(panelType,handle);
            panelData.State = PanelState.Loaded;
        }
        
        public T GetPanel<T>()where T : IPanel
        {
            return default;
        }

        public T OpenPanel<T>()where T : IPanel
        {
            var panelType = typeof(T);
            if (_panelAssetDic.TryGetValue(panelType, out var handle))
            {
                var go = handle.AssetObject as GameObject;
                if (go != null && go.TryGetComponent<T>(out var panel))
                {
                    var layer = panel.Layer;
                    var layerRoot = _uiLayerDic[layer];
                    GameObject.Instantiate(go,layerRoot);
                    panel.OnOpen();
                    return panel;
                }
            }
            Debug.LogError($"{panelType} is not Loaded");
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
            
        }
    }
}