using UnityEngine;
using UnityEngine.UI;

namespace Native.UIKit.Framework
{
    public abstract class UIPanel :MonoBehaviour,IPanel
    {
        public UILayer Layer { get;protected set; }
        public virtual void OnOpen()
        {
            
        }

        public virtual void OnClose()
        {
            
        }
        
        public virtual void OnFocus()
        {
            
        }

        public virtual void OnUnfocus()
        {
            
        }
    }
}