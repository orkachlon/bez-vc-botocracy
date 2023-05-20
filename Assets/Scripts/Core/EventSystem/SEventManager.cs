using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.EventSystem {
    [CreateAssetMenu(menuName = "Events/Event Manager", order = 0)]
    public class SEventManager : ScriptableObject {

        [SerializeField] private bool debugMessages;
        
        private readonly Dictionary<string, Action<EventParams>> _eventMap = new();

        public virtual void Register(string eventName, Action<EventParams> listener) {
            if (_eventMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent += listener;
                _eventMap[eventName] = thisEvent;
            }
            else {
                thisEvent += listener;
                _eventMap.Add(eventName, thisEvent);
            }
        }

        public virtual void Unregister(string eventName, Action<EventParams> listener) {
            if (!_eventMap.TryGetValue(eventName, out var thisEvent))
                return;
            thisEvent -= listener;
            _eventMap[eventName] = thisEvent;
        }

        public virtual void Raise(string eventName, EventParams eventParams) {
            if (_eventMap.TryGetValue(eventName, out var thisEvent)) {
                thisEvent?.Invoke(eventParams);
            }

            if (debugMessages) {
                MonoBehaviour.print($"{name} Raised {eventName}!");
            }
        }
    }

    public class EventParams {
        
    }
}