using UnityEngine;

namespace MyHexBoardSystem.UI {
    public interface IHoverEffect {
        void Show(Vector3 position);
        void Hide();
    }
}