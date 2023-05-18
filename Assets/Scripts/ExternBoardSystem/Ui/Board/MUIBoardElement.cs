using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;

namespace ExternBoardSystem.Ui.Board {
    
    /// <summary>
    ///     A board element's visual appearance. This is also used as a container for pooling board elements.
    /// </summary>
    public class MUIBoardElement : MonoBehaviour {
        private BoardElement RuntimeData { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }
        private Transform Transform { get; set; }

        protected virtual void Awake() {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

            Transform = transform;
        }

        public void SetRuntimeElementData(BoardElement data) {
            RuntimeData = data;
            UpdateView();
        }

        public void SetWorldPosition(Vector3 position) {
            Transform.position = position;
        }

        private void UpdateView() {
            SpriteRenderer.sprite = RuntimeData.DataProvider.GetArtwork();
        }
    }
}