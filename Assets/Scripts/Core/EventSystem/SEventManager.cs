using System;
using System.Collections.Generic;
using Types.Events;
using UnityEngine;

namespace Core.EventSystem {
    [CreateAssetMenu(menuName = "Events/Event Manager", order = 0)]
    public class SEventManager : ScriptableObject, IEventManager {

        [SerializeField] private bool debugMessages;
        
        private readonly Dictionary<string, Action<EventArgs>> _eventActionMap = new();

        public virtual void Register(string eventName, Action<EventArgs> listener) {
            if (_eventActionMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent += listener;
                _eventActionMap[eventName] = thisEvent;
            }
            else {
                thisEvent += listener;
                _eventActionMap.Add(eventName, thisEvent);
            }
        }

        public virtual void Unregister(string eventName, Action<EventArgs> listener) {
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
        }
    }
}