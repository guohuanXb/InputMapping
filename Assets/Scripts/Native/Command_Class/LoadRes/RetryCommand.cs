using Native.Event_Class;
using VFramework;

namespace Native
{
    public class RetryCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<RetryEvent>();
        }
    }
}