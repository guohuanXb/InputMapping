using YooAsset;

namespace Native.UIKit.Framework
{
    public enum PanelState
    {
        /// <summary>
        /// 未加载
        /// </summary>
        UnLoad,
        /// <summary>
        /// 已加载
        /// </summary>
        Loaded,   
        /// <summary>
        /// 已实例化
        /// </summary>
        Instantiated,
    }
    
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
        public PanelData(PanelState state, PanelConfig config,bool isOpened = false ,bool isFocus = false)
        {
            State = state;
            Config = config;
            IsOpened = isOpened;
            IsFocus = isFocus;
            Handle = null;
            Panel = null;
        }

        public PanelState State { get; set; }
        
        public bool IsFocus { get; set; }
        public bool IsOpened { get; set; }
        public PanelConfig Config { get; set;}
        public AssetHandle Handle { get; set;}
        public IPanel Panel { get; set;}
    }
}