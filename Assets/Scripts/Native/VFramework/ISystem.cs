namespace VFramework
{
    public interface ISystem :ISetArchitecture,IGetModel,IGetUtility,IRegisterEvent,ISendEvent,IGetSystem,IInit
    {
        
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


        public bool Initialized { get; set; }
        void IInit.Init() => OnInit();

        public void Deinit() => OnDeinit();
        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }
}