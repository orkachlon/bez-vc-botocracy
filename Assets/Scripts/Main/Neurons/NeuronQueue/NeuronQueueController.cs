using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using JetBrains.Annotations;
using Main.GameModifications;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using UnityEngine;

namespace Main.Neurons.NeuronQueue {
    public class NeuronQueueController : MonoBehaviour, IEnumerable<BoardNeuron> {

        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager modificationsEventManager;

        public int Count => _isInfinite ? int.MaxValue : _neurons.Count;

        private Queue<BoardNeuron> _neurons;
        private bool _isInfinite;

        private void Awake() {
            _neurons = new Queue<BoardNeuron>();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElement, OnBoardElementPlaced);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, OnBoardElementPlaced);
            neuronEventManager.Unregister(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Unregister(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
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

            // never run out of neurons but keep visibility and functionality the same
            if (_isInfinite) {
                Enqueue(1);
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

        private void OnInfiniteNeurons(EventArgs eventArgs) {
            if (eventArgs is not IsInfiniteNeuronsEventArgs infiniteNeurons) {
                return;
            }

            _isInfinite = infiniteNeurons.IsInfinite;
        }

        #endregion

        public IEnumerator<BoardNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}