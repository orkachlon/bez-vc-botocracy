using UnityEngine;

namespace Neurons {
    public class MUINeuron : MonoBehaviour {
        private Neuron RuntimeData { get; set; }
        private SpriteRenderer SpriteRenderer { get; set; }
        private Transform Transform { get; set; }
        
        public Neuron.ENeuronType Type { get; set; }

        protected virtual void Awake() {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

            Transform = transform;
        }

        public void SetRuntimeElementData(Neuron data) {
            RuntimeData = data;
            UpdateView();
        }

        public void SetWorldPosition(Vector3 position) {
            Transform.position = position;
        }

        private void UpdateView() {
            SpriteRenderer.sprite = RuntimeData.NeuronData.GetArtwork();
        }
        
        public void Hide() {
            SpriteRenderer.enabled = false;
        }

        public void Show() {
            SpriteRenderer.enabled = true;
        }
    }
}