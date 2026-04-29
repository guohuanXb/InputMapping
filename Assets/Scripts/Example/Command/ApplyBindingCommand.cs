using Framework;

namespace Example
{
    public class ApplyBindingCommand : AbstractCommand
    {
        public string ActionName;
        public string NewBindingPath;

        protected override void OnExecute()
        {
            var model = this.GetModel<IInputMappingModel>();
            model.GetBinding(ActionName).Value = NewBindingPath;

            var storage = this.GetUtility<IStorage>();
            storage.SaveString("InputBinding_" + ActionName, NewBindingPath);

            this.SendEvent(new BindingChangedEvent
            {
                ActionName = ActionName,
                NewBindingPath = NewBindingPath
            });
        }
    }
}
