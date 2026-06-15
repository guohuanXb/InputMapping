using System.Collections.Generic;
using VFramework;

namespace Native
{
    public interface IDllModel :IModel
    {
        List<string> AotAllName { get; }
        string HotUpdateDllName { get; }
    }

    public class DllModel :AbstractModel,IDllModel
    {
        protected override void OnInit()
        {
            AotAllName = new()
            {
                "Unity.InputSystem",
                "UnityEngine.CoreModule",
                "VFramework",
                "mscorlib"
            };
            HotUpdateDllName = "HotUpdate";
        }

        public List<string> AotAllName { get; private set; }
        public string HotUpdateDllName { get; private set; }
    }
}