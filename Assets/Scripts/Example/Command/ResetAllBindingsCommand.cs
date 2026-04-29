using Framework;

namespace Example
{
    public class ResetAllBindingsCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var system = this.GetSystem<IPlayerInputSystem>();
            system.ResetAllBindingOverrides();

            var model = this.GetModel<IInputMappingModel>();
            var storage = this.GetUtility<IStorage>();

            foreach (var actionName in model.ActionNames)
            {
                string defaultPath = system.GetDefaultPath(actionName);
                model.GetBinding(actionName).Value = defaultPath;
                storage.SaveString("InputBinding_" + actionName, defaultPath);
            }
        }
    }
}
