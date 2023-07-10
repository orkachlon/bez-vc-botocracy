using System;
using System.Collections.Concurrent;
using Types.Events;
using UnityEngine;

namespace Core.EventSystem {
    [CreateAssetMenu(menuName = "Events/Event Manager", order = 0)]
    public class SEventManager : ScriptableObject, IEventManager {

        [SerializeField] private bool debugMessages;
        
        private readonly ConcurrentDictionary<string, Action<EventArgs>> _eventActionMap = new();
        private readonly ConcurrentDictionary<string, Action<EventArgs>> _dontDestroyActions = new();

        public virtual void Register(string eventName, Action<EventArgs> listener, bool dontDestroyOnLoad = false) {
            if (dontDestroyOnLoad) {
                RegisterDontDestroyOnLoad(eventName, listener);
            }
            RegisterDestroyOnLoad(eventName, listener);
        }

        protected virtual void RegisterDontDestroyOnLoad(string eventName, Action<EventArgs> listener) {
            if (_dontDestroyActions.TryGetValue(eventName, out var thisEvent)) {
                thisEvent += listener;
                _dontDestroyActions[eventName] = thisEvent;
            }
            else {
                thisEvent += listener;
                _dontDestroyActions.TryAdd(eventName, thisEvent);
            }
        }

        protected virtual void RegisterDestroyOnLoad(string eventName, Action<EventArgs> listener) {
            if (_eventActionMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent += listener;
                _eventActionMap[eventName] = thisEvent;
            }
            else {
                thisEvent += listener;
                _eventActionMap.TryAdd(eventName, thisEvent);
            }
        }

        public virtual void Unregister(string eventName, Action<EventArgs> listener, bool dontDestroyOnLoad = false) {
            UnregisterDestroyOnLoad(eventName, listener);
            if (dontDestroyOnLoad) {
                UnregisterDontDestroyOnLoad(eventName, listener);
            }
        }
        
        protected virtual void UnregisterDontDestroyOnLoad(string eventName, Action<EventArgs> listener) {
            if (!_dontDestroyActions.TryGetValue(eventName, out var thisEvent))
                return;
            thisEvent -= listener;
            _dontDestroyActions[eventName] = thisEvent;
        }

        protected virtual void UnregisterDestroyOnLoad(string eventName, Action<EventArgs> listener) {
            if (!_eventActionMap.TryGetValue(eventName, out var thisEvent))
                return;
            thisEvent -= listener;
            _eventActionMap[eventName] = thisEvent;
        }

        public virtual void Raise(string eventName, EventArgs eventParams) {
            if (_eventActionMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent?.Invoke(eventParams);
            }

            if (debugMessages) {
                MonoBehaviour.print($"{name} Raised {eventName}!");
            }
        }

        public virtual void BindToScene(string eventName) {
            Register(eventName, ClearAllListeners);
        }

        public virtual void UnbindFromScene(string eventName) {
            Unregister(eventName, ClearAllListeners);
        }

        private void ClearAllListeners(EventArgs _) {
            _eventActionMap.Clear();
            foreach (var (eventString, action) in _dontDestroyActions) {
                _eventActionMap[eventString] = action;
            }
        }
    }
}