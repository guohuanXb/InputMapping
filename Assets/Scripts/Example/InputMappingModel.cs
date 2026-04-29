using System.Collections.Generic;
using System.Linq;
using Framework;

namespace Example
{
    public interface IInputMappingModel : IModel
    {
        BindableProperty<string> GetBinding(string actionName);
        bool HasBinding(string actionName);
        IEnumerable<string> ActionNames { get; }
    }

    public class InputMappingModel : AbstractModel, IInputMappingModel
    {
        private Dictionary<string, BindableProperty<string>> _bindings = new();

        protected override void OnInit() { }

        public BindableProperty<string> GetBinding(string actionName)
        {
            if (!_bindings.TryGetValue(actionName, out var prop))
            {
                prop = new BindableProperty<string>();
                _bindings[actionName] = prop;
            }
            return _bindings[actionName];
        }

        public bool HasBinding(string actionName)
        {
            return _bindings.ContainsKey(actionName);
        }

        public IEnumerable<string> ActionNames => _bindings.Keys.ToList();
    }
}
