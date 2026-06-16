using Example.System_Class;
using UnityEngine;
using VFramework;

namespace Example.Query_Class
{
    public class BindingNameQuery :AbstractQuery<string>
    {
        private string _name;
        private int _bindIndex;
        public BindingNameQuery(string name,int bindIndex)
        {
            _name = name;
            _bindIndex = bindIndex;
        }

        protected override string OnDo()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            return system.GetBindingDisplayString(_name,_bindIndex);
        }
    }
}