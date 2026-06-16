using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Controller_Class.Component
{
    public class ControlMappingComponent : MonoBehaviour
    {
        public Button button;
        public TMP_Text content;
        
        public string actionName;
        public int bindingIndex;
        
        public void SetDisplay(string text) => content.text = text;
        public void ClearDisplay() => content.text = "";
    }
}