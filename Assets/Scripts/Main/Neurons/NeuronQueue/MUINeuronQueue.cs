using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Main.Neurons.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Neurons.NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {

        [SerializeField] private NeuronQueueController controller;
        [SerializeField, Range(3, 10)] private int neuronsToShow = 7;
        [SerializeField] private TextMeshProUGUI neuronCountDisplay;
        [SerializeField] private VerticalLayoutGroup stack, top3;
        [SerializeField] private MUINeuron uiNeuronPrefab;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        

        private readonly List<MUINeuron> _registerUiElements = new ();

        private void Awake() {
            InitializeEmptyQueue();
        }

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Register(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Unregister(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        private void InitializeEmptyQueue() {
            for (var i = 0; i < 3; i++) {
                var model = Instantiate(uiNeuronPrefab, top3.transform, true);
                _registerUiElements.Add(model);
                model.transform.SetSiblingIndex(i);
                model.SetPlaceInQueue(neuronsToShow - 1 - i);
                model.gameObject.SetActive(false);
            }

            for (var i = 0; i < neuronsToShow - 3; i++) {
                var model = Instantiate(uiNeuronPrefab, stack.transform, true);
                _registerUiElements.Add(model);
                model.transform.SetSiblingIndex(i);
                model.SetPlaceInQueue(neuronsToShow - 4 - i);
                model.gameObject.SetActive(false);
            }
        }

        private void OnEnqueue(Neuron neuron) {
            neuronCountDisplay.text = $"{controller.Count}";
            if (_registerUiElements.All(n => n.gameObject.activeInHierarchy)) {
                return;
            }
            
            ShowNeuron(neuron);
        }

        private void OnDequeue(Neuron neuron) {
            neuronCountDisplay.text = $"{controller.Count}";
            ShiftNeuronsInQueue();

            // show the next neuron in queue
            var lastNeuron = controller.Peek(neuronsToShow - 1);
            
            // we have less than 'neuronsToShow' neurons
            if (lastNeuron == null) {
                return;
            }
            ShowNeuron(lastNeuron);
        }

        private void ShowNeuron(Neuron neuron) {
            var uiElement = _registerUiElements.First(n => !n.gameObject.activeInHierarchy);
            uiElement.gameObject.SetActive(true);
            neuron.UIState = ENeuronUIState.Stack;
            uiElement.SetRuntimeElementData(neuron);
        }

        private void ShiftNeuronsInQueue() {
            for (var i = 0; i < _registerUiElements.Count - 1; i++) {
                var uin = _registerUiElements[i];
                var nextUin = _registerUiElements[i + 1];
                if (i < 3) {
                    nextUin.RuntimeData.UIState = (ENeuronUIState) i;
                }
                else {
                    nextUin.RuntimeData.UIState = ENeuronUIState.Stack;
                }
                uin.SetRuntimeElementData(nextUin.RuntimeData);
            }
            _registerUiElements[^1].gameObject.SetActive(false);
        }

        #region EventHandlers

        private void OnEnqueue(EventArgs eventData) {
            if (eventData is UINeuronEventArgs neuronData) {
                OnEnqueue(neuronData.UINeuron);
            }
        }
        
        private void OnDequeue(EventArgs eventData) {
            if (eventData is UINeuronEventArgs neuronData) {
                OnDequeue(neuronData.UINeuron);
            }
        }

        #endregion
    }
}