using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Core.EventSystem;
using Events.Neuron;
using TMPro;
using Types.Neuron;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;

namespace Neurons.NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {
        [SerializeField, Range(3, 10)] private int neuronsToShow = 7;
        [SerializeField] private TextMeshProUGUI neuronCountDisplay;
        [SerializeField] private RectTransform stack;
        [SerializeField] private int stackSpacing = 100, top3Spacing = 150;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        private const string InfiniteNeuronsMark = "-";
        private readonly Queue<KeyValuePair<IStackNeuron, IUIQueueNeuron>> _registerUiElements = new ();

        private Canvas _queueCanvas;

        public float StackSpacing => stackSpacing;
        public float Top3Spacing => top3Spacing;


        private void Awake() {
            _queueCanvas = GetComponent<Canvas>();
        }

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
            var refNeuron = _registerUiElements.Dequeue();
            await refNeuron.Value.AnimateDequeue();
            refNeuron.Key.Release();
        }

        private void ShowNeuron(IStackNeuron stackNeuron) {
            var uiElement = stackNeuron.Pool(stack);
            uiElement.Default();
            // can't use the isntantiate overload with position because
            // parenting is done after setting position 
            uiElement.GO.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            // scale the entire neuron with the canvas in order to fit to different screen sizes
            uiElement.GO.transform.localScale *= _queueCanvas.scaleFactor;

            var placeInQueue = _registerUiElements.Count;
            _registerUiElements.Enqueue(new KeyValuePair<IStackNeuron, IUIQueueNeuron>(stackNeuron, uiElement));
            stackNeuron.SetPlaceInQueue(placeInQueue);
            uiElement.SetRuntimeElementData(stackNeuron);
            SetQueuePosition(uiElement, placeInQueue);
        }

        private void SetQueuePosition(IUIQueueNeuron uiElement, int placeInQueue) {
            uiElement.GO.transform.SetAsFirstSibling();
            if (placeInQueue < 3) {
                uiElement.SetQueuePosition(placeInQueue * top3Spacing);
                return;
            }
            uiElement.SetQueuePosition((top3Spacing * 2) + (placeInQueue - 1) * stackSpacing);
        }

        private async void ShiftNeuronsInQueue() {
            var shiftTasks = new List<Task>();
            var queueAsArray = _registerUiElements.ToArray();
            for (var i = 0; i < _registerUiElements.Count; i++) {
                var neuron = queueAsArray[i];
                shiftTasks.Add(neuron.Value.AnimateQueueShift(i, -stackSpacing, -top3Spacing));
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