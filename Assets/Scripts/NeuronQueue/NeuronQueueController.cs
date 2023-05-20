using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.Events;
using ExternBoardSystem.Tools;
using JetBrains.Annotations;
using Managers;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class NeuronQueueController : MonoBehaviour, IEnumerable<BoardNeuron> {

        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager boardEventManager;

        public int Count => _neurons.Count;

        private Queue<BoardNeuron> _neurons;

        private void Awake() {
            _neurons = new Queue<BoardNeuron>();
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElement, OnBoardElementPlaced);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
        }

        public void Enqueue(IEnumerable<BoardNeuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(BoardNeuron neuron) {
            _neurons.Enqueue(neuron);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronEventArgs(neuron));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(NeuronManager.GetRandomNeuron());
            }
        }
            
        public BoardNeuron Dequeue() {
            if (_neurons.Count == 0) {
                neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronEventArgs());
                return null;
            }
            _neurons.TryDequeue(out var nextNeuron);
            if (nextNeuron == null) {
                return null;
            }
            neuronEventManager.Raise(NeuronEvents.OnDequeueNeuron, new NeuronEventArgs(nextNeuron));
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

        private void OnBoardElementPlaced(EventArgs eventData) {
            if (eventData is OnPlaceElementEventArgs<BoardNeuron> neuronEventData) {
                Dequeue();
            }
        }

        private void OnRewardNeurons(EventArgs eventArgs) {
            if (eventArgs is NeuronRewardEventArgs reward) {
                Enqueue(reward.Amount);
            }
        }

        #endregion

        public IEnumerator<BoardNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}