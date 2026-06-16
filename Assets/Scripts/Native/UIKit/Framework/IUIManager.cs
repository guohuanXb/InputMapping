using VFramework;

namespace Native.UIKit.Framework
{
    public interface IUIManager: ISystem
    {
        T OpenPanel<T>(string prefabName = null, UILevel level = UILevel.Common, IUIData uiData = null);
        void ClosePanel<T>() where T : UIPanel;
        void ShowPanel<T>() where T : UIPanel;
        void HidePanel<T>() where T : UIPanel;
        T GetPanel<T>() where T : UIPanel;
    }
}