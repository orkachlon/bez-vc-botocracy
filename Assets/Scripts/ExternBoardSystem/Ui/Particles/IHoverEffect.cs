using UnityEngine;

namespace ExternBoardSystem.Ui.Particles {
    public interface IHoverEffect {
        void Show(Vector3 position);
        void Hide();
    }
}