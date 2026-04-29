using System;
using System.Collections.Generic;

namespace Framework
{
    public class IOCContainer
    {
        public Dictionary<Type, Object> Instances = new();

        public void Register<T>(T instance) where T :class
        {
            var key = typeof(T);
            if(Instances.ContainsKey(key))
            {
                Instances[key] = instance;
            }
            else
            {
                Instances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);
            if(Instances.TryGetValue(key, out var instance))
                return (T)instance;
            return null;
        }
    }
}