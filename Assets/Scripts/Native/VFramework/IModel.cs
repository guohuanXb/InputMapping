namespace VFramework
{
    public interface IModel :ISetArchitecture,IGetUtility,ISendEvent,IInit
    {
        
    }

    public abstract class AbstractModel : IModel
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