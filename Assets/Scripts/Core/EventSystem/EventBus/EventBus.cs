using System.Collections.Generic;
using Types.Events;

namespace Core.EventSystem.EventBus {
    public static class EventBus<T> where T : IEvent {
        private static readonly HashSet<IEventBinding<T>> bindings = new();

        public static void Register(EventBinding<T> binding) => bindings.Add(binding);
        public static void Unregister(EventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event) {
            foreach (var binding in bindings) {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        static void Clear() {
            bindings.Clear();
        }
    }
}
