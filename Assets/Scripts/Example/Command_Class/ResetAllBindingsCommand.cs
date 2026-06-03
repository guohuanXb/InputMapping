using Example.System_Class;
using VFramework;

namespace Example.Command_Class
{
    public class ResetAllBindingsCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            system.ResetAllBindingOverrides();
        }
    }
}
