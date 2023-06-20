﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Neuron;
using Neurons.UI;
using TMPro;
using Types.Neuron;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Neurons.NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {

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

        private void Enqueue(INeuronQueue neuronQueue) {
            SetNeuronCounterText(neuronQueue.IsInfinite, neuronQueue.Count);
            
            if (!neuronQueue.IsInfinite && neuronQueue.Count > neuronsToShow) {
                return;
            }

            if (neuronQueue.IsInfinite && _registerUiElements.TrueForAll(n => n.gameObject.activeInHierarchy)) {
                return;
            }
            
            ShowNeuron(neuronQueue.PeekLast());
        }

        private void Dequeue(INeuronQueue neuronQueue) {
            SetNeuronCounterText(neuronQueue.IsInfinite, neuronQueue.Count);
            ShiftNeuronsInQueue();
            HideExcessNeurons(neuronQueue);

            // show the next neuron in queue
            var lastNeuron = neuronQueue.Peek(neuronsToShow - 1);
            
            // we have less than 'neuronsToShow' neurons
            if (lastNeuron == null) {
                return;
            }
            ShowNeuron(lastNeuron);
        }

        private void HideExcessNeurons(INeuronQueue neuronQueue) {
            var excess = neuronsToShow - neuronQueue.Count;
            if (excess <= 0) {
                return;
            }

            for (var i = 1; i <= excess; i++) {
                _registerUiElements[^i].gameObject.SetActive(false);
            }
        }

        private void ShowNeuron(IStackNeuron stackNeuron) {
            var uiElement = _registerUiElements.First(n => !n.gameObject.activeInHierarchy);
            uiElement.gameObject.SetActive(true);
            var placeInQueue = _registerUiElements.IndexOf(uiElement);
            if (placeInQueue >= 3) {
                stackNeuron.UIState = ENeuronUIState.Stack;
            }
            else {
                stackNeuron.UIState = (ENeuronUIState) placeInQueue;
            }
            uiElement.SetRuntimeElementData(stackNeuron);
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

        private void SetNeuronCounterText(bool isInfinite, int amount = 0) {
            neuronCountDisplay.text = isInfinite ? "-" :$"{amount}";
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