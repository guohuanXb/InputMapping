using Framework;

namespace Example
{
    /// <summary>
    /// Note: This command stores a single path per (action, bindingName).
    /// For composite rebinding (e.g. 2DVector Move), use
    /// PlayerInputSystem.BeginRebind() which saves all composite parts
    /// via SaveActionOverrides.
    /// </summary>
    public class ApplyBindingCommand : AbstractCommand
    {
        public string ActionName;
        public string BindingName;
        public string NewBindingPath;

        protected override void OnExecute()
        {
            var storage = this.GetUtility<IStorage>();
            storage.SaveString($"InputBinding_{ActionName}_{BindingName}", NewBindingPath);

            var model = this.GetModel<IInputMappingModel>();
            var bindingData = model.GetBindingData(ActionName, BindingName);
            if (bindingData != null)
            {
                bindingData.OverridePath.Value = NewBindingPath;
            }

            this.SendEvent(new BindingChangedEvent
            {
                ActionName = ActionName,
                BindingName = BindingName,
                NewBindingPath = NewBindingPath
            });
        }
    }
}
