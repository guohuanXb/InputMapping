using Framework;

namespace Example
{
    public class ResetBindingCommand : AbstractCommand
    {
        public string ActionName;

        protected override void OnExecute()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            string defaultPath = system.GetDefaultPath(ActionName);

            var model = this.GetModel<IInputMappingModel>();
            model.GetBinding(ActionName).Value = defaultPath;

            var storage = this.GetUtility<IStorage>();
            storage.SaveString("InputBinding_" + ActionName, defaultPath);

            this.SendEvent(new BindingChangedEvent
            {
                ActionName = ActionName,
                NewBindingPath = defaultPath
            });
        }
    }
}
