namespace VFramework
{
    public interface ICommand :ISetArchitecture,IGetSystem,IGetModel,IGetUtility,ISendEvent,ISendCommand,ISendQuery
    {
        void Execute();
    }

    public interface ICommand<TResult> : ISetArchitecture, IGetSystem, IGetModel, IGetUtility, ISendEvent, ISendCommand,
        ISendQuery
    {
        TResult Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture _architecture;

        IArchitecture IGetArchitecture.GetArchitecture()
        {
            return _architecture;
        }

        void ISetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            _architecture = architecture;
        }

        void ICommand.Execute()
        {
            OnExecute();
        }

        protected abstract void OnExecute();
    }
    
    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        private IArchitecture _architecture;

        IArchitecture IGetArchitecture.GetArchitecture()
        {
            return _architecture;
        }

        void ISetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            _architecture = architecture;

        }

        TResult ICommand<TResult>.Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }
}
