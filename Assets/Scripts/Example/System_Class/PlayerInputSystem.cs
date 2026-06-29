using System.Collections.Generic;
using Example.Event_Class;
using Example.Utility_Class;
using UnityEngine.InputSystem;
using VFramework;

namespace Example.System_Class
{
    public interface IPlayerInputSystem : ISystem
    {
        bool IsRebinding { get; }
        void BeginRebind(string actionName);
        void BeginRebind(string actionName, int bindingIndex);
        void CancelRebind();
        void ResetBindingOverride(string actionName, int bindingIndex);
        void ResetAllBindingOverrides();
        void SaveOverrides();

        int GetBindingCount(string actionName);
        string GetBindingDisplayString(string actionName, int bindingIndex);

        InputAction GetInputAction(string actionName);
        IEnumerable<InputAction> GetAllInputAction();
    }

    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem
    {
        private const string InputActionAssetPath = "OperationControl";
        private const string BindingJsonKey = "BindingOverrides";
        private const string PcInputMapName = "PC";
        private InputActionMap _actionMap;
        private InputActionAsset _actionAsset;
        private InputActionRebindingExtensions.RebindingOperation _currentRebind;
        private string _currentRebindActionName;
        private int _currentRebindBindingIndex;
        public bool IsRebinding => _currentRebind != null;
        protected override void OnInit()
        {
            _actionAsset = this.GetUtility<IResourcesStore>()
                .LoadInputActionAsset<InputActionAsset>(InputActionAssetPath);
            _actionMap = _actionAsset.FindActionMap(PcInputMapName);
            LoadSavedOverrides();
        }
        
        private void LoadSavedOverrides()
        {
            var saved = this.GetUtility<IStorage>().LoadString(BindingJsonKey);
            if (!string.IsNullOrEmpty(saved))
            {
                _actionAsset.RemoveAllBindingOverrides();
                _actionAsset.LoadBindingOverridesFromJson(saved);
            }
        }

        public void BeginRebind(string actionName)
        {
            BeginRebind(actionName, 0);
        }

        public void BeginRebind(string actionName, int bindingIndex)
        {
            if (IsRebinding) return;

            var action = _actionMap.FindAction(actionName);
            if (action == null) return;
            if (bindingIndex < 0 || bindingIndex >= action.bindings.Count) return;
            
            this.SendEvent(new BeginRebindEvent(actionName, bindingIndex));
            
            _currentRebindActionName = actionName;
            _currentRebindBindingIndex = bindingIndex;
            action.Disable();
            
            _currentRebind = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(OnRebindComplete)
                .OnCancel(OnRebindCancel)
                .Start();
        }

        private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            var action = _actionMap.FindAction(_currentRebindActionName);
            var newPath = operation.selectedControl.path;

            action.ApplyBindingOverride(_currentRebindBindingIndex, new InputBinding { overridePath = newPath });
            action.Enable();

            var display = GetBindingDisplayString(_currentRebindActionName, _currentRebindBindingIndex);
            this.SendEvent(new RebindCompleteEvent(_currentRebindActionName,_currentRebindBindingIndex,
                display));

            CleanupRebind();
            SaveOverrides();
        }

        private void OnRebindCancel(InputActionRebindingExtensions.RebindingOperation operation)
        {
            var action = _actionMap.FindAction(_currentRebindActionName);
            action?.Enable();
            CleanupRebind();
            this.SendEvent(new CancelRebindEvent(_currentRebindActionName,_currentRebindBindingIndex));
        }

        private void CleanupRebind()
        {
            _currentRebind?.Dispose();
            _currentRebind = null;
        }

        public void CancelRebind()
        {
            _currentRebind?.Cancel();
        }

        public void ResetBindingOverride(string actionName, int bindingIndex)
        {
            var action = _actionMap.FindAction(actionName);
            if (action == null) return;

            action.RemoveBindingOverride(bindingIndex);

            var display = GetBindingDisplayString(actionName, bindingIndex);
            this.SendEvent(new RebindCompleteEvent(actionName,bindingIndex,display));

            SaveOverrides();
        }

        public void ResetAllBindingOverrides()
        {
            _actionAsset.RemoveAllBindingOverrides();
            SaveOverrides();
        }

        public void SaveOverrides()
        {
            var json = _actionAsset.SaveBindingOverridesAsJson();
            this.GetUtility<IStorage>().SaveString(BindingJsonKey, json);
        }

        public int GetBindingCount(string actionName)
        {
            var action = _actionMap.FindAction(actionName);
            return action?.bindings.Count ?? 0;
        }

        public string GetBindingDisplayString(string actionName, int bindingIndex)
        {
            var action = _actionMap.FindAction(actionName);
            if (action == null || bindingIndex >= action.bindings.Count)
                return string.Empty;

            var path = action.bindings[bindingIndex].effectivePath;
            return InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.UseShortNames);
        }

        public InputAction GetInputAction(string actionName)
        {
            foreach (var action in _actionMap)
            {
                if (action.name == actionName)
                    return action;
            }

            return null;
        }

        public IEnumerable<InputAction> GetAllInputAction()
        {
            return _actionMap.actions;
        }
    }
}
