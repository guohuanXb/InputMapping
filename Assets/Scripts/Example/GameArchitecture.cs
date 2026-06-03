using Example.Model_Class;
using Example.System_Class;
using Example.Utility_Class;
using VFramework;

namespace Example
{
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        protected override void Init()
        {
            RegisterUtility<IStorage>(new PlayerPrefsStore());
            RegisterUtility<IResourcesStore>(new ResourcesStorage());
            RegisterModel<IInputMappingModel>(new InputMappingModel());
            RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
        }
    }
}
