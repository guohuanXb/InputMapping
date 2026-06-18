namespace Native.UIKit.Framework
{
    public enum PanelState
    {
        /// <summary>
        /// 未加载
        /// </summary>
        UnLoad,
        /// <summary>
        /// 已加载，但未打开
        /// </summary>
        Loaded,     
        /// <summary>
        /// 打开状态
        /// </summary>
        Opened,   
        /// <summary>
        /// 关闭状态
        /// </summary>
        Closed,     
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
        public PanelData(PanelState state, PanelConfig config, bool isFocus)
        {
            State = state;
            Config = config;
            IsFocus = isFocus;
        }

        public PanelState State { get; set; }
        public PanelConfig Config { get; set;}
        public bool IsFocus { get; set; }
        
    }
}