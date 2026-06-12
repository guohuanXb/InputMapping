namespace VFramework
{
    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }
    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.Register<T>(self.OnEvent);

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
    }
}