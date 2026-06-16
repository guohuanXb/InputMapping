namespace Example.Event_Class
{
    public struct RebindCompleteEvent
    {
        public string ActionName;
        public int BindingIndex;
        public string DisplayContent;

        public RebindCompleteEvent(string actionName, int bindingIndex, string displayContent)
        {
            ActionName = actionName;
            BindingIndex = bindingIndex;
            DisplayContent = displayContent;
        }
    }
}