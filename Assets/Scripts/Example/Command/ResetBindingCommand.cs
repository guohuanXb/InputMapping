using Framework;

namespace Example
{
    public class ResetBindingCommand : AbstractCommand
    {
        public string ActionName;

        protected override void OnExecute()
        {
            var model = this.GetModel<IInputMappingModel>();
            var bindingData = model.GetBindingData(ActionName);

            var system = this.GetSystem<IPlayerInputSystem>();
            system.ResetBindingOverride(ActionName);

            this.SendEvent(new BindingChangedEvent
            {
                ActionName = ActionName,
                BindingName = bindingData?.BindingName,
                NewBindingPath = bindingData?.DefaultPath
            });
        }
    }
}
