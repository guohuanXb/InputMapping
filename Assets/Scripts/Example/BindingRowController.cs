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

        public IArchitecture GetArchitecture() => GameArchitecture.Interface;

        public void Setup(string actionName)
        {
            _actionName = actionName;
            actionLabel.text = actionName;

            var system = this.GetSystem<IPlayerInputSystem>();
            bindingText.text = system.GetBindingDisplayName(actionName);

            var model = this.GetModel<IInputMappingModel>();
            model.GetBinding(actionName).OnValueChanged += OnBindingChanged;

            rebindButton.onClick.AddListener(OnRebindClick);
            //resetButton.onClick.AddListener(OnResetClick);
        }

        private void OnBindingChanged(string newPath)
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
            var model = this.GetModel<IInputMappingModel>();
            if (model.HasBinding(_actionName))
            {
                model.GetBinding(_actionName).OnValueChanged -= OnBindingChanged;
            }
        }
    }
}
