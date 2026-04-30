using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Example
{
    public interface IPlayerInputSystem : ISystem
    {
        void Initialize(InputActionMap map);
        string GetActionDisplayName(string actionName);
        string GetBindingDisplayName(string actionName,int  bindingIndex);
        void BeginRebind(string actionName,int  bindingIndex);
        void CancelRebind();
        bool IsRebinding { get; }
        string CurrentRebindingAction { get; }
        void ResetAllBindingOverrides();
        void ResetBindingOverride(string actionName);
    }

    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem
    {
        private InputActionMap _map;
        private Dictionary<string, InputAction> _actions = new();
        private InputActionRebindingExtensions.RebindingOperation _currentRebindOp;
        private string _currentRebindingAction;
        private bool _actionMapWasEnabled;

        public bool IsRebinding => _currentRebindOp != null;
        public string CurrentRebindingAction => _currentRebindingAction;

        protected override void OnInit() { }

       

        
    }
}
