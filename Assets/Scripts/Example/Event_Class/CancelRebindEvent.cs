namespace Example.Event_Class
{
    public struct CancelRebindEvent
    {
        public string ActionName;
        public int BindingIndex;

        public CancelRebindEvent(string actionName, int bindingIndex)
        {
            ActionName = actionName;
            BindingIndex = bindingIndex;
        }
    }
}