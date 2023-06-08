using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Neurons.UI {
    public class MUINeuron : MonoBehaviour {
        public Neuron RuntimeData { get; set; }
        
        protected Image UIImage { get; set; }
        protected Transform Transform { get; set; }
        protected Canvas Canvas { get; set; }
        
        protected virtual void Awake() {
            UIImage = GetComponent<Image>();
            Transform = transform;
            Canvas = GetComponent<Canvas>();
        }
        
        public virtual void SetRuntimeElementData(Neuron data) {
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