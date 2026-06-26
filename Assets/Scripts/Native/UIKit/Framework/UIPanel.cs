using UnityEngine;
using UnityEngine.UI;

namespace Native.UIKit.Framework
{
    public abstract class UIPanel :MonoBehaviour,IPanel
    {
        public UILayer Layer { get;protected set; }
        public virtual void OnInit()
        {
            
        }

        public virtual void OnOpen()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnClose()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void OnFocus()
        {
            
        }

        public virtual void OnUnfocus()
        {
            
        }
    }
}