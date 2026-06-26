using System;
using Native.Event_Class;
using Native.UIKit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

namespace Native
{
    public class ErrorPanel :MonoBehaviour,IController
    {
        public Button retryButton;
        public TMP_Text errorText;
        public IArchitecture GetArchitecture()
            => GameManager.Interface;
        private IUnRegister _unRegister;
        private void UpdateErrorText(string error)
        {
            errorText.text = error;
        }

        private void OnEnable()
        {
            _unRegister = this.RegisterEvent<UpdateExceptionEvent>(e => UpdateErrorText(e.Error));
            retryButton.onClick.AddListener(() =>
            {
                this.SendCommand(new RetryCommand());
            });
        }

        private void OnDisable()
        {
            _unRegister.UnRegister();
            retryButton.onClick.RemoveAllListeners();
        }
    }
}