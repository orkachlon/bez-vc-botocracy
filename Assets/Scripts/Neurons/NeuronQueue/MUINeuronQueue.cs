using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Core.EventSystem;
using Events.Neuron;
using TMPro;
using Types.Neuron;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;

namespace Neurons.NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {
        private const string InfiniteNeuronsMark = "-";
        [SerializeField, Range(3, 10)] private int neuronsToShow = 7;
        [SerializeField] private TextMeshProUGUI neuronCountDisplay;
        [SerializeField] private RectTransform stack;
        [SerializeField] private float stackSpacing = 100, top3Spacing = 150;
        [SerializeField] private MUINeuron uiNeuronPrefab;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        

        private readonly List<MUINeuron> _registerUiElements = new ();

        public float StackSpacing => stackSpacing;
        public float Top3Spacing => top3Spacing;


        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Register(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Unregister(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        private void Enqueue(INeuronQueue neuronQueue) {
            SetNeuronCounterText(neuronQueue.IsInfinite, neuronQueue.Count);
            
            if (!neuronQueue.IsInfinite && neuronQueue.Count > neuronsToShow) {
                return;
            }

            if (neuronQueue.IsInfinite && _registerUiElements.Count >= neuronsToShow) {
                return;
            }
            
            ShowNeuron(neuronQueue.PeekLast());
        }

        private void Dequeue(INeuronQueue neuronQueue) {
            SetNeuronCounterText(neuronQueue.IsInfinite, neuronQueue.Count);
            DequeueNeuron();
            ShiftNeuronsInQueue();

            // show the next neuron in queue
            var lastNeuron = neuronQueue.Peek(neuronsToShow - 1);

            // we have less than 'neuronsToShow' neurons
            if (lastNeuron == null) {
                return;
            }
            ShowNeuron(lastNeuron);
        }

        private async void DequeueNeuron() {
            var refNeuron = _registerUiElements[0];
            _registerUiElements.Remove(refNeuron);
            await refNeuron.AnimateDequeue();
            Destroy(refNeuron.gameObject);
        }


        private void ShowNeuron(IStackNeuron stackNeuron) {
            var uiElement = Instantiate(uiNeuronPrefab, stack);
            // can't use the isntantiate overload with position because
            // parenting is done after setting position 
            uiElement.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            var placeInQueue = _registerUiElements.Count;
            _registerUiElements.Add(uiElement);
            stackNeuron.PlaceInQueue = placeInQueue;
            uiElement.SetRuntimeElementData(stackNeuron);
            SetQueuePosition(uiElement, placeInQueue);
        }

        private void SetQueuePosition(MUINeuron uiElement, int placeInQueue) {
            uiElement.transform.SetAsFirstSibling();
            if (placeInQueue < 3) {
                uiElement.SetQueuePosition(placeInQueue * top3Spacing);
                return;
            }
            uiElement.SetQueuePosition((top3Spacing * 2) + (placeInQueue - 1) * stackSpacing);
        }

        private async void ShiftNeuronsInQueue() {
            var shiftTasks = new List<Task>();
            for (var i = 0; i < _registerUiElements.Count; i++) {
                MUINeuron neuron = _registerUiElements[i];
                shiftTasks.Add(neuron.AnimateQueueShift(i));
            }
            await Task.WhenAll(shiftTasks);
        }

        private void SetNeuronCounterText(bool isInfinite, int amount = 0) {
            neuronCountDisplay.text = isInfinite ? InfiniteNeuronsMark : $"{amount}";
        }

        #region EventHandlers

        private void OnEnqueue(EventArgs eventData) {
            if (eventData is NeuronQueueEventArgs neuronData) {
                Enqueue(neuronData.NeuronQueue);
            }
        }
        
        private void OnDequeue(EventArgs eventData) {
            if (eventData is NeuronQueueEventArgs neuronData) {
                Dequeue(neuronData.NeuronQueue);
            }
        }

        #endregion
    }
}