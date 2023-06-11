using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using JetBrains.Annotations;
using Main.GameModifications;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Interfaces;
using UnityEngine;

namespace Main.Neurons.NeuronQueue {
    public class MNeuronQueue : MonoBehaviour, INeuronQueue, IEnumerable<Neuron> {
        
        [Header("Current Neuron Data"), SerializeField]
        private SNeuronData currentNeuronData;

        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager modificationsEventManager;

        public int Count => _isInfinite ? int.MaxValue : _neurons.Count;

        private Queue<Neuron> _neurons;
        private bool _isInfinite;

        #region UnityMethods

        private void Awake() {
            _neurons = new Queue<Neuron>();
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

        #endregion

        public void Enqueue(IEnumerable<Neuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(Neuron neuron) {
            _neurons.Enqueue(neuron);
            currentNeuronData.SetData(Peek().DataProvider);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronQueueEventArgs(this));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(new Neuron(NeuronManager.GetRandomNeuron()));
            }
        }
        
        public Neuron Dequeue() {
            _neurons.TryDequeue(out var nextNeuron);
            if (nextNeuron == null) {
                currentNeuronData.Type = ENeuronType.Undefined; // shouldn't happen, just to be sure
                return null;
            }
            // never run out of neurons but keep visibility and functionality the same
            if (_isInfinite) {
                Enqueue(1);
            }
            neuronEventManager.Raise(NeuronEvents.OnDequeueNeuron, new NeuronQueueEventArgs(this));
            
            if (_neurons.Count == 0) {
                currentNeuronData.Type = ENeuronType.Undefined;
                neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronQueueEventArgs(this));
                return null;
            }
            currentNeuronData.SetData(Peek().DataProvider);
            return nextNeuron;
        }

        public Neuron Peek() {
            return Count == 0 ? null : _neurons.Peek();
        }

        public Neuron PeekLast() {
            return _neurons.ToArray()[_neurons.Count - 1];
        }

        public Neuron[] PeekFirst(int number) {
            if (number > _neurons.Count) {
                number = _neurons.Count;
            }

            return _neurons.ToArray()[Range.EndAt(number)];
        }

        [CanBeNull]
        public Neuron Peek(int index) {
            if (0 <= index && index < _neurons.Count) {
                return _neurons.ToArray()[index];
            }

            return null;
        }

        #region EventHandlers

        private void OnBoardElementPlaced(EventArgs eventData) {
            if (eventData is BoardElementEventArgs<BoardNeuron> neuronEventData) {
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

        public IEnumerator<Neuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}