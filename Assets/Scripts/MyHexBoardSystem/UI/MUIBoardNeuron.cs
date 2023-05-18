using ExternBoardSystem.BoardElements;
using MyHexBoardSystem.BoardElements.Neuron;
using UnityEngine;

namespace MyHexBoardSystem.UI {
    public class MUIBoardNeuron : MonoBehaviour {
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