using Example.Model_Class;
using Example.System_Class;
using Example.Utility_Class;
using Native;
using VFramework;

namespace Example
{
    public static class HotUpdateEntry
    {
        public static IArchitecture Architecture => GameManager.Interface;
        public static void DependencyRegister()
        {
            Architecture.RegisterUtility<IStorage>(new PlayerPrefsStore());
            Architecture.RegisterUtility<IResourcesStore>(new ResourcesStorage());
            Architecture.RegisterModel<IInputMappingModel>(new InputMappingModel());
            Architecture.RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
        }
    }
}
