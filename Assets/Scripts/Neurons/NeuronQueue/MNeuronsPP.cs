using System;
using Core.EventSystem;
using Events.Neuron;
using PostProcessing;
using UnityEngine;

namespace Neurons.NeuronQueue {
    public class MNeuronsPP : MonoBehaviour {

        [SerializeField] private Color neuronsCriticalColor;
        [SerializeField] private MVignetteController vignetteController;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        
        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnNeuronAmountLow, ChangeToCriticalColor);
            neuronEventManager.Register(NeuronEvents.OnNeuronAmountStable, ChangeToStableColor);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnNeuronAmountLow, ChangeToCriticalColor);
            neuronEventManager.Unregister(NeuronEvents.OnNeuronAmountStable, ChangeToStableColor);
        }

        private void ChangeToStableColor(EventArgs obj) {
            vignetteController.SetDefaults(true);
        }

        private void ChangeToCriticalColor(EventArgs obj) {
            vignetteController.SetValues(neuronsCriticalColor, 0, 1.8f, true);
        }
    }
}