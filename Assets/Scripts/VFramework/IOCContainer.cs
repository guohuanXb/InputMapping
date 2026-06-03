using System;
using System.Collections.Generic;
using System.Linq;

namespace VFramework
{
    public class IOCContainer
    {
        private Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            if (_instances.ContainsKey(key))
            {
                _instances[key] = instance;
            }
            else
            {
                _instances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (_instances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public IEnumerable<T> GetInstancesByType<T>()
        {
            var type = typeof(T);
            return _instances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => _instances.Clear();
    }
}