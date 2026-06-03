using Example.System_Class;
using VFramework;

namespace Example.Command_Class
{
    public class ResetBindingCommand : AbstractCommand
    {
        public string ActionName;
        public int BindingIndex;

        protected override void OnExecute()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            system.ResetBindingOverride(ActionName, BindingIndex);
        }
    }
}
