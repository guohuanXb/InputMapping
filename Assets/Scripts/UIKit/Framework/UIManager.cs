using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native.UIKit.Framework
{
    public class UIManager :AbstractSystem,IUIManager
    {
        private IResourceSystem _resourceSystem;
        private Dictionary<Type,AssetHandle> _panelAssetDic;
        private Dictionary<Type,PanelData> _panelDic;
        protected override void OnInit()
        {
            _resourceSystem = new YooAssetResourceSystem();
            _panelAssetDic = new();
            _panelDic = new();
            LoadAllPanelData();
            
        }

        private void LoadAllPanelData()
        {
            //TODO:读取Config
            List<PanelConfig> configs = new();
            foreach (var config in configs)
            {
                var panelData = new PanelData(state: PanelState.UnLoad, config: config, isFocus: false);
                
                _panelDic.Add();
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
            
        }

        public T OpenPanel<T>()where T : IPanel
        {
            var panelType = typeof(T);
            if (_panelAssetDic.TryGetValue(panelType, out var handle))
            {
                var go = handle.AssetObject as GameObject;
                if (go != null && go.TryGetComponent<T>(out var panel))
                {
                    GameObject.Instantiate(go,);
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