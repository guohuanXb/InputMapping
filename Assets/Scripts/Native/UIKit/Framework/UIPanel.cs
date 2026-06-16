using UnityEngine;
using UnityEngine.UI;

namespace Native.UIKit.Framework
{
    public interface IUIData
    {
        
    }

    public enum UILevel
    {
        Common,
        Background,
    }
    
    public abstract class UIPanel :MonoBehaviour
    {
        protected IUIData UIData;

        public UILevel Level { get; set; }

        internal void Init(IUIData data = null)
        {
            UIData = data;
            OnInit(data);
            OnOpen(data);
            OnShow();
        }
        
        protected virtual void OnInit(IUIData data = null) { }
        protected virtual void OnOpen(IUIData data = null) { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected virtual void OnClose() { }
    }
}