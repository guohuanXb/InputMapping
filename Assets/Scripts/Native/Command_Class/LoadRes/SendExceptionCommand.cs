using Native.Event_Class;
using VFramework;

namespace Native
{
    public class SendExceptionCommand :AbstractCommand
    {
        private string _errorMessage;

        public SendExceptionCommand(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        protected override void OnExecute()
        {
            this.SendEvent(new UpdateExceptionEvent()
            {
                Error = _errorMessage
            });
        }
    }
}