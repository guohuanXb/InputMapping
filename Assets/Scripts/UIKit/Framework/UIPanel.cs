using UnityEngine;
using UnityEngine.UI;

namespace Native.UIKit.Framework
{
    public enum UILayer
    {
        First = 0,
        Second = 10,
        Third = 20,
        Fourth = 30,
        Fifth = 40,
    }
    
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