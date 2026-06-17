using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;

namespace Native.UIKit.Framework
{
    public interface IUIManager : ISystem
    {
        UniTask LoadPanelAsync<T>() where T : IPanel;
        T GetPanel<T>() where T : IPanel;
        T OpenPanel<T>() where T : IPanel;
        UniTask<T> OpenPanelAsync<T>() where T : IPanel;
        void FocusPanel(IPanel panel);
        void ClosePanel<T>() where T : IPanel;
    }
}