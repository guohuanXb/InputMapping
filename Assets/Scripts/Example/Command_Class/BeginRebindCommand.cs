using Example.System_Class;
using VFramework;

namespace Example.Command_Class
{
    public class BeginRebindCommand : AbstractCommand
    {
        private string _actionName;
        private int _bindingIndex;
        public BeginRebindCommand(string actionName, int bindingIndex)
        {
            _actionName = actionName;
            _bindingIndex = bindingIndex;
        }

        protected override void OnExecute()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            system.BeginRebind(_actionName, _bindingIndex);
        }
    }
}