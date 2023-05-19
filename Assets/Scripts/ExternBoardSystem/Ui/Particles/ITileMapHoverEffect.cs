using UnityEngine;

namespace ExternBoardSystem.Ui.Particles {
    
    /// <summary>
    ///     Basic interface for tile hover effect.
    /// </summary>
    public interface IHoverEffect {
        void Show(Vector3 position);
        void Hide();
    }
}