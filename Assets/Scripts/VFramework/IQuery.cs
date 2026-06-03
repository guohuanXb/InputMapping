namespace VFramework
{
    public interface IQuery<TResult> : ISetArchitecture,IGetModel,IGetSystem,ISendQuery
    {
        TResult Do();
    }
    
    
    
    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do() => OnDo();

        protected abstract T OnDo();


        private IArchitecture _architecture;

        IArchitecture IGetArchitecture.GetArchitecture()
        {
            return _architecture;
        }

        void ISetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            _architecture = architecture;
        }
    }

}