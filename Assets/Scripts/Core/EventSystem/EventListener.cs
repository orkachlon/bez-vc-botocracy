using UnityEngine;
using UnityEngine.Events;

namespace Core.EventSystem {
    public abstract class EventListener<T> : MonoBehaviour {

        [SerializeField] SEventChannel<T> eventChannel;
        [SerializeField] UnityEvent<T> unityEvent;

        public void Raise(T value) {
            unityEvent?.Invoke(value);
        }

        private void Awake() {
            if (eventChannel == null) {
                return;
            }
            eventChannel.Register(this);
        }

        private void OnDestroy() {
            if (eventChannel == null) {
                return;
            }
            eventChannel.Unregister(this);
        }
    }
}