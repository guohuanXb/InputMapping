namespace Framework
{
    public interface ISystem :ISetArchitecture,IGetModel,IGetUtility,IRegisterEvent,ISendEvent
    {
        void Init();
    }
    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture _architecture = null;
        IArchitecture IGetArchitecture.GetArchitecture() 
        {
            return _architecture;
        }

        void ISetArchitecture.SetArchitecture(IArchitecture architecture) 
        {
            _architecture = architecture;
        }


        void ISystem.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }
}