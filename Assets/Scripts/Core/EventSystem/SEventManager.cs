using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.EventSystem {
    [CreateAssetMenu(menuName = "Events/Event Manager", order = 0)]
    public class SEventManager : ScriptableObject {

        [SerializeField] private bool debugMessages;
        
        private readonly Dictionary<string, Action<EventArgs>> _eventActionMap = new();
        private readonly Dictionary<string, Func<EventArgs, EventArgs>> _eventFunctionMap = new();

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
        
        public virtual void Raise(string eventName, ref EventArgs eventParams) {
            if (_eventFunctionMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent?.Invoke(eventParams);
            }

            if (debugMessages) {
                MonoBehaviour.print($"{name} Raised {eventName}!");
            }
        }
    }
}