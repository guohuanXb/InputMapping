using System.Collections.Generic;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
    public class BindingRowController : MonoBehaviour, IController
    {
        [SerializeField] private TMP_Text actionLabel;
        [SerializeField] private TMP_Text bindingText;
        [SerializeField] private Button rebindButton;
        //[SerializeField] private Button resetButton;

        private string _actionName;
        private List<BindableProperty<string>> _subscribedOverrides = new();

        public IArchitecture GetArchitecture() => GameArchitecture.Interface;

        public void Setup(string actionName)
        {
            _actionName = actionName;
            actionLabel.text = actionName;

            var system = this.GetSystem<IPlayerInputSystem>();
            bindingText.text = system.GetBindingDisplayName(actionName);

            var model = this.GetModel<IInputMappingModel>();
            foreach (var bindingData in model.GetAllBindingDataForAction(actionName))
            {
                bindingData.OverridePath.OnValueChanged += OnOverrideChanged;
                _subscribedOverrides.Add(bindingData.OverridePath);
            }

            rebindButton.onClick.AddListener(OnRebindClick);
            //resetButton.onClick.AddListener(OnResetClick);
        }

        private void OnOverrideChanged(string newPath)
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            bindingText.text = system.GetBindingDisplayName(_actionName);
        }

        private void OnRebindClick()
        {
            bindingText.text = "——";
            var system = this.GetSystem<IPlayerInputSystem>();
            if (system.IsRebinding) return;
            system.BeginRebind(_actionName);
        }

        private void OnResetClick()
        {
            this.SendCommand(new ResetBindingCommand { ActionName = _actionName });
        }

        private void OnDestroy()
        {
            foreach (var prop in _subscribedOverrides)
            {
                prop.OnValueChanged -= OnOverrideChanged;
            }
            _subscribedOverrides.Clear();
        }
    }
}
