using Framework;

namespace Example
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
