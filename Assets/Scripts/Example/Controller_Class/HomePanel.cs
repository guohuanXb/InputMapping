using Native;
using Native.UIKit.Framework;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

namespace Example.Controller_Class
{
    public class HomePanel: UIPanel,IController
    {
        public override UILayer Layer { get; protected set; } = UILayer.First;
        public IArchitecture GetArchitecture() => GameManager.Interface;
        public Button startBtn;
        public Button exitBtn;

        public override void OnOpen()
        {
            startBtn.onClick.AddListener(() =>
            {
                this.SendCommand(new OpenPanelCommand<InputMappingPanel>());
                this.SendCommand(new ClosePanelCommand<HomePanel>());
            });
            exitBtn.onClick.AddListener(() =>
            {
                #if  UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            });
        }

        public override void OnClose()
        {
            startBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}