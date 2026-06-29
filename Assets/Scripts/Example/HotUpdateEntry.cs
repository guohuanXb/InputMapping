using System.Collections.Generic;
using Example.Controller_Class;
using Example.Model_Class;
using Example.System_Class;
using Example.Utility_Class;
using Native;
using Native.Event_Class;
using Native.UIKit.Framework;
using UnityEngine;
using VFramework;

namespace Example
{
    public static class HotUpdateEntry
    {
        public static IArchitecture Architecture => GameManager.Interface;
        public static void DependencyRegister()
        {
            Architecture.RegisterUtility<IStorage>(new PlayerPrefsStore());
            Architecture.RegisterUtility<IResourcesStore>(new ResourcesStorage());
            Architecture.RegisterModel<IInputMappingModel>(new InputMappingModel());
            Architecture.RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
            
            // 监听场景就绪事件，在场景加载完成后打开默认面板
            Architecture.RegisterEvent<SceneReadyEvent>(OnSceneReady);
            RegisterScenePanels();
        }

        private static void RegisterScenePanels()
        {
            // 注册 Main 场景需要的面板
            var uiManager = Architecture.GetSystem<IUIManager>();
            uiManager.RegisterScenePanels("Main", new List<string>
            {
                "HomePanel",
                "InputMappingPanel"
            });
        }


        private static void OnSceneReady(SceneReadyEvent e)
        {
            if (e.SceneName == "Main")
            {
                Architecture.SendCommand(new OpenPanelCommand<HomePanel>());
            }
        }
    }
}
