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
    public interface IPanel
    {
        UILayer Layer { get; }
        void OnInit();
        void OnOpen();
        void OnClose();
        void OnFocus();
        void OnUnfocus();
    }
}