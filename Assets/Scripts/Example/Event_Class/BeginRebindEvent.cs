using System;
using VFramework;

namespace Example.Event_Class
{
    public struct BeginRebindEvent 
    {
        public string ActionName;
        public int BindingIndex;

        public BeginRebindEvent(string actionName,int bindingIndex)
        {
            ActionName = actionName;
            BindingIndex = bindingIndex;
        }
    }
}