using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.Tools;
using JetBrains.Annotations;
using Managers;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class NeuronQueueController : MonoBehaviour, IEnumerable<BoardNeuron> {

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        public int Count => _neurons.Count;

        private Queue<BoardNeuron> _neurons;

        private void Awake() {
            _neurons = new Queue<BoardNeuron>();
            
            neuronEventManager.Register(NeuronEvents.OnRequestNeuronFromQueue, OnRequestNeuronFromQueue);
        }

        public void Enqueue(IEnumerable<BoardNeuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(BoardNeuron neuron) {
            _neurons.Enqueue(neuron);
            // OnEnqueueNeuron?.Invoke(neuron);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronEvent(neuron));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(NeuronManager.GetRandomNeuron());
            }
        }
            
        public BoardNeuron Dequeue() {
            if (_neurons.Count == 0) {
                neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronEvent(null));
                return null;
            }
            _neurons.TryDequeue(out var nextNeuron);
            if (nextNeuron == null) {
                return null;
            }
            // OnDequeueNeuron?.Invoke(nextNeuron);
            neuronEventManager.Raise(NeuronEvents.OnDequeueNeuron, new NeuronEvent(nextNeuron));
            return nextNeuron;
        }

        public BoardNeuron Peek() {
            return _neurons.Peek();
        }

        public BoardNeuron PeekLast() {
            return _neurons.ToArray()[_neurons.Count - 1];
        }

        [CanBeNull]
        public BoardNeuron Peek(int index) {
            if (0 <= index && index < _neurons.Count) {
                return _neurons.ToArray()[index];
            }

            return null;
        }

        #region EventHandlers

        private void OnRequestNeuronFromQueue(EventParams eventData) {
            if (eventData is NeuronEvent neuronEventData) {
                neuronEventData.Neuron = Dequeue();
            }
        }

        #endregion

        public IEnumerator<BoardNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}