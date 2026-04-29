using Framework;

namespace Example
{
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        protected override void Init()
        {
            RegisterModel<IInputMappingModel>(new InputMappingModel());
            RegisterUtility<IStorage>(new PlayerPrefsStore());
            RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
        }
    }
}
