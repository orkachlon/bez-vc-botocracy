using System.Collections.Generic;
using UnityEngine;

namespace Core.EventSystem {
    public abstract class SEventChannel<T> : ScriptableObject {

        readonly HashSet<EventListener<T>> _observers = new();

        public void Invoke(T value) {
            foreach (var listener in _observers) {
                listener.Raise(value);
            }
        }

        public void Register(EventListener<T> observer) => _observers.Add(observer);
        public void Unregister(EventListener<T> observer) => _observers.Remove(observer);


        public void Clear() => _observers.Clear();
    }
}