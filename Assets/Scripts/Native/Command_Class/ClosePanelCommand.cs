using Native.UIKit.Framework;
using VFramework;

namespace Native
{
    public class ClosePanelCommand<T> :AbstractCommand where T : IPanel
    {
        protected override void OnExecute()
        {
            var uiManager = this.GetSystem<IUIManager>();
            uiManager.ClosePanel<T>();
        }
    }
}