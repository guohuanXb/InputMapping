using System.Collections.Generic;
using Framework;
using UnityEngine.InputSystem;

namespace Example
{

    public class BindingData
    {
        public string ActionName { get;private set; }
        public string BindingName { get; private set; }
        
        public int BindingIndex { get; private set; }
        
        public BindableProperty<string> BindingPath { get;set; }

        public BindingData(string actionName, string bindingName, int bindingIndex ,string bindingPath)
        {
            ActionName = actionName;
            BindingName = bindingName;
            BindingIndex = bindingIndex;
            BindingPath = new(){Value = bindingPath};
        }
    }
    
    

    public interface IInputMappingModel : IModel
    {
        void SetBindingData(BindingData data);
        BindingData GetBindingData(string actionName, int bindingIndex);
        IEnumerable<BindingData> GetAllBindingDataForAction(string actionName);
    }

    public class InputMappingModel : AbstractModel, IInputMappingModel
    {
        protected override void OnInit() { }

        private List<BindingData> _bindings = new List<BindingData>();
        public void SetBindingData(BindingData data)
        {
            _bindings.Add(data);
        }

        public BindingData GetBindingData(string actionName, int bindingIndex)
        {
            return _bindings.Find(b => b.ActionName == actionName && b.BindingIndex == bindingIndex);
        }

        public IEnumerable<BindingData> GetAllBindingDataForAction(string actionName)
        {
            return _bindings.FindAll(b => b.ActionName == actionName);
        }
    }
}
