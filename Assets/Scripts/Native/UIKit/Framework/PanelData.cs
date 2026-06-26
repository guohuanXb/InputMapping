using YooAsset;

namespace Native.UIKit.Framework
{
    
    public class PanelConfig
    {
        public string PackageName;
        /// <summary>
        /// 约定使用PanelPrefab的Name作为Location
        /// </summary>
        public string Location;
    }
    
    public class PanelData
    {
        public PanelData(bool isOpened = false ,bool isFocus = false)
        {
            IsOpened = isOpened;
            IsFocus = isFocus;
            Panel = null;
        }
        public bool IsFocus { get; set;}
        public bool IsOpened { get; set;}
        public IPanel Panel { get; set;}
    }
}