namespace VFramework
{
    public interface IInit
    {
        bool Initialized { get; set; }
        void Init();
        void Deinit();
    }
}