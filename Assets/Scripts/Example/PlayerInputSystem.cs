using System.Collections.Generic;
using Framework;
using UnityEngine.InputSystem;

namespace Example
{
    public interface IPlayerInputSystem : ISystem
    {
        void Initialize(InputActionMap map);
        string GetBindingDisplayName(string actionName);
        string GetDefaultPath(string actionName);
        void BeginRebind(string actionName);
        void CancelRebind();
        bool IsRebinding { get; }
        string CurrentRebindingAction { get; }
        void ResetAllBindingOverrides();
    }

    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem
    {
        private InputActionMap _map;
        private Dictionary<string, InputAction> _actions = new();
        private Dictionary<string, string> _defaultPaths = new();
        private InputActionRebindingExtensions.RebindingOperation _currentRebindOp;
        private string _currentRebindingAction;
        private bool _actionMapWasEnabled;

        public bool IsRebinding => _currentRebindOp != null;
        public string CurrentRebindingAction => _currentRebindingAction;

        protected override void OnInit()
        {
            
        }

        public void Initialize(InputActionMap map)
        {
            _map = map;
            foreach (var action in map)
            {
                string actionName = action.name;
                _actions[actionName] = action;
                    
                if (action.bindings.Count > 0 && !string.IsNullOrEmpty(action.bindings[0].effectivePath))
                {
                    _defaultPaths[actionName] = action.bindings[0].effectivePath;
                }
                    
                var model = this.GetModel<IInputMappingModel>();
                var bindingProp = model.GetBinding(actionName);

                var storage = this.GetUtility<IStorage>();
                string savedPath = storage.LoadString("InputBinding_" + actionName);

                if (!string.IsNullOrEmpty(savedPath))
                {
                    action.ApplyBindingOverride(0, savedPath);
                    bindingProp.Value = savedPath;
                }
                else if (_defaultPaths.TryGetValue(actionName, out var defaultPath) && !string.IsNullOrEmpty(defaultPath))
                {
                    bindingProp.Value = defaultPath;
                }
            }
        }

        public string GetBindingDisplayName(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
            {
                return action.GetBindingDisplayString(0);
            }
            return actionName;
        }

        public string GetDefaultPath(string actionName)
        {
            return _defaultPaths.GetValueOrDefault(actionName, "");
        }

        public void BeginRebind(string actionName)
        {
            if (IsRebinding) return;
            if (!_actions.TryGetValue(actionName, out var action)) return;

            _currentRebindingAction = actionName;
            _actionMapWasEnabled = action.actionMap.enabled;
            if (_actionMapWasEnabled)
            {
                action.actionMap.Disable();
            }

            this.SendEvent(new RebindStateChangedEvent
            {
                ActionName = actionName,
                IsRebinding = true
            });

            _currentRebindOp = action.PerformInteractiveRebinding(0)
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Gamepad>")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(op =>
                {
                    string newPath = op.selectedControl.path;
                    op.Dispose();
                    _currentRebindOp = null;
                    _currentRebindingAction = null;

                    var model = this.GetModel<IInputMappingModel>();
                    model.GetBinding(actionName).Value = newPath;
                    var storage = this.GetUtility<IStorage>();
                    storage.SaveString("InputBinding_" + actionName, newPath);

                    this.SendEvent(new RebindStateChangedEvent
                    {
                        ActionName = actionName,
                        IsRebinding = false
                    });

                    if (_actionMapWasEnabled)
                    {
                        action.actionMap.Enable();
                    }
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    _currentRebindOp = null;
                    _currentRebindingAction = null;

                    this.SendEvent(new RebindStateChangedEvent
                    {
                        ActionName = actionName,
                        IsRebinding = false
                    });

                    if (_actionMapWasEnabled)
                    {
                        action.actionMap.Enable();
                    }
                })
                .Start();
        }

        public void CancelRebind()
        {
            _currentRebindOp?.Cancel();
            _currentRebindOp?.Dispose();
            _currentRebindOp = null;
            _currentRebindingAction = null;
        }

        public void ResetAllBindingOverrides()
        {
            foreach (var kvp in _actions)
            {
                kvp.Value.RemoveBindingOverride(0);
            }
        }
    }
}
