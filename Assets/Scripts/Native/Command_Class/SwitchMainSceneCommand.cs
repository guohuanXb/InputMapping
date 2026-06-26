using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Native.UIKit.Framework;
using VFramework;
using YooAsset;

namespace Native
{
    public class SwitchMainSceneCommand :AbstractCommand<UniTask>
    {
        private string _packageName;
        private string _sceneName;

        public SwitchMainSceneCommand(string packageName, string sceneName)
        {
            _packageName = packageName;
            _sceneName = sceneName;
        }

        protected override async UniTask OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var config = new SceneConfig(_sceneName){SuspendLoad =  true};
            var sceneHandle = await resourceSystem.LoadSceneAsync(_packageName,config);
            try
            {
                if (sceneHandle.Status == EOperationStatus.Failed)
                {
                    throw new InvalidOperationException($"Load Scene {_sceneName} failed");
                }
                var uiManager = this.GetSystem<IUIManager>();
                sceneHandle.ActivateScene();
                var list = new List<string>()
                {
                    "Panel",
                };
                await uiManager.PreloadPanelForScene(_packageName,list);
                await uiManager.InstantiateLayer();
            }
            finally
            {
                sceneHandle.Release();
            }
        }


        
    }
}