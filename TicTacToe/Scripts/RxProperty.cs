using System;

namespace TicTacToe
{
    public class RxProperty<T> where T : struct
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed?.Invoke(value);
            }
        }

        private event Action<T> Changed;

        public RxProperty()
        {
        }

        public RxProperty(T value)
        {
            _value = value;
        }

        public void Subscribe(Action<T> action)
        {
            Changed += action;
        }

        public void UnSubscribe(Action<T> action)
        {
            Changed -= action;
        }

        public void UnSubscribeAll()
        {
            Changed = null;
        }

        public static implicit operator T(RxProperty<T> rxProperty)
        {
            return rxProperty._value;
        }
    }
}