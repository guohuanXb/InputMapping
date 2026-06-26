using Native.UIKit.Framework;
using VFramework;

namespace Native
{
    public class OpenPanelCommand<T> :AbstractCommand<T> where T : UIPanel
    {
        protected override T OnExecute()
        {
            var uiManager = this.GetSystem<IUIManager>();
            return uiManager.OpenPanel<T>();
        }
    }
}