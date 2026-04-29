using System;

namespace Framework
{
    public class BindableProperty<T> where T :IEquatable<T>
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (_value == null || !_value.Equals(value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }
        public Action<T> OnValueChanged;
    }
}
