using UnityEngine;

namespace MyHexBoardSystem {
    public interface IHoverEffect {
        void Show(Vector3 position);
        void Hide();
    }
}