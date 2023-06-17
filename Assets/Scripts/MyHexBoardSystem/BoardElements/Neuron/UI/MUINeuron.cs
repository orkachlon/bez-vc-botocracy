using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Neurons.UI {
    public class MUINeuron : MonoBehaviour, IUINeuron {
        public Types.Neuron.Runtime.IStackNeuron RuntimeData { get; set; }
        protected Image UIImage { get; set; }
        protected Transform Transform { get; set; }
        protected Canvas Canvas { get; set; }
        
        protected virtual void Awake() {
            UIImage = GetComponent<Image>();
            Transform = transform;
            Canvas = GetComponent<Canvas>();
        }
        
        public virtual void SetRuntimeElementData(Types.Neuron.Runtime.IStackNeuron data) {
            RuntimeData = data;
            UpdateView();
        }

        protected virtual void UpdateView() {
            UIImage.sprite = RuntimeData.DataProvider.GetUIArtwork(RuntimeData.UIState);
        }

        public void SetPlaceInQueue(int place) {
            Canvas.sortingOrder = place;
        }
    }

}