using UnityEngine;

namespace Types.Pooling {
    public interface IPoolable {
        public GameObject GO { get; }
        void Default();
    }
}