namespace Native.UIKit.Framework
{
    public interface IPanel
    {
        UILayer Layer { get; }
        void OnOpen();
        void OnClose();
        void OnFocus();
        void OnUnfocus();
    }
}