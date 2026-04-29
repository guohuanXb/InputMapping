namespace Framework
{
    public interface ICommand :ISetArchitecture,IGetSystem,IGetModel,IGetUtility,ISendEvent,ISendCommand
    {
        void Execute();
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
}
