using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.Neuron;
using TMPro;
using Types.Neuron;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Neurons.NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {
        [SerializeField] private Image bg;
        [SerializeField, Range(3, 10)] private int neuronsToShow = 7;
        [SerializeField] private TextMeshProUGUI neuronCountDisplay;
        [SerializeField] private RectTransform stack;
        [SerializeField] private int stackSpacing = 100, top3Spacing = 150, topPadding = -50;
        [SerializeField] private float enqueueShakeStrength;
        
        [Header("Event Managers"), SerializeField]
        protected SEventManager neuronEventManager;

        private const string InfiniteNeuronsMark = "-";
        private readonly Queue<IStackNeuron> _registerUiElements = new ();

        private Color _baseColor;

        public float StackSpacing => stackSpacing;
        public float Top3Spacing => top3Spacing;

        private void Awake() {
            _baseColor = bg.color;
        }

        protected virtual void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Register(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        protected virtual void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnEnqueueNeuron, OnEnqueue);
            neuronEventManager.Unregister(NeuronEvents.OnDequeueNeuron, OnDequeue);
        }

        private void Enqueue(INeuronQueue neuronQueue) {
            SetNeuronCounterText(neuronQueue.IsInfinite, neuronQueue.Count);
            AnimateBG(neuronQueue.IsInfinite);
            
            if (!neuronQueue.IsInfinite && neuronQueue.Count > neuronsToShow) {
                return;
            }

            if (neuronQueue.IsInfinite && _registerUiElements.Count >= neuronsToShow) {
                return;
            }
            
            ShowNeuron(neuronQueue.PeekLast());
        }

        private void AnimateBG(bool isInfinite) {
            if (isInfinite) {
                return;
            }
            bg.color = Color.white;
            bg.DOColor(_baseColor, 0.5f);
            bg.rectTransform.DOShakePosition(0.5f, Vector3.up * enqueueShakeStrength,
                randomness: 0, fadeOut: true, randomnessMode:ShakeRandomnessMode.Harmonic);
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
            await refNeuron.PlayDequeueAnimation();
            refNeuron.Release();
        }

        private void ShowNeuron(IStackNeuron stackNeuron) {
            var uiElement = stackNeuron.Pool(stack);
            uiElement.Default();
            // can't use the instantiate overload with position because
            // parenting is done after setting position 
            uiElement.GO.GetComponent<RectTransform>().anchoredPosition = Vector3.up * top3Spacing;

            var placeInQueue = _registerUiElements.Count;
            _registerUiElements.Enqueue(stackNeuron);
            stackNeuron.SetPlaceInQueue(placeInQueue);
            uiElement.SetRuntimeElementData(stackNeuron);
            SetQueuePosition(uiElement, placeInQueue);
        }

        private void SetQueuePosition(IUIQueueNeuron uiElement, int placeInQueue) {
            uiElement.GO.transform.SetAsFirstSibling();
            if (placeInQueue < 3) {
                uiElement.SetQueuePosition(placeInQueue * top3Spacing - topPadding);
                return;
            }
            uiElement.SetQueuePosition((top3Spacing * 2) + (placeInQueue - 1) * stackSpacing - topPadding);
        }

        private async void ShiftNeuronsInQueue() {
            var shiftTasks = new List<Task>();
            foreach (var neuron in _registerUiElements) {
                shiftTasks.Add(neuron.PlayQueueShiftAnimation(-stackSpacing, -top3Spacing));
            }
            await Task.WhenAll(shiftTasks);
        }

        private void SetNeuronCounterText(bool isInfinite, int amount = 0) {
            if (isInfinite) {
                neuronCountDisplay.text = InfiniteNeuronsMark;
                return;
            }
            var currentAmount = int.Parse(neuronCountDisplay.text);
            DOVirtual.Int(currentAmount, amount, 0.3f * Mathf.Abs(amount - currentAmount),
                i => neuronCountDisplay.text = $"{i}");
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