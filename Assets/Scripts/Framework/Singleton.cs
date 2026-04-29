using System;
using System.Reflection;

namespace Framework
{
    public class Singleton<T> where T :new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                    if (ctor == null)
                    {
                        throw new Exception("Non-public parameterless constructor not found for " + typeof(T).Name);
                    }
                    _instance = (T)ctor.Invoke(null);
                }

                return _instance;
            }
        }
    }
}
