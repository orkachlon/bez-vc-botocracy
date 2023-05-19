using System;
using System.Collections;
using System.Collections.Generic;
using ExternBoardSystem.Tools;
using JetBrains.Annotations;
using Managers;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class NeuronQueueController : MonoBehaviour, IEnumerable<BoardNeuron> {

        public int Count => _neurons.Count;
        
        public event Action<BoardNeuron> OnEnqueueNeuron;
        public event Action<BoardNeuron> OnDequeueNeuron;

        private Queue<BoardNeuron> _neurons;

        private void Awake() {
            _neurons = new Queue<BoardNeuron>();
        }

        public void Enqueue(IEnumerable<BoardNeuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(BoardNeuron neuron) {
            _neurons.Enqueue(neuron);
            OnEnqueueNeuron?.Invoke(neuron);
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(NeuronManager.Instance.GetRandomNeuron());
            }
        }
            
        public BoardNeuron Dequeue() {
            if (_neurons.Count == 0) {
                return null;
            }
            _neurons.TryDequeue(out var nextNeuron);
            if (nextNeuron == null) {
                return null;
            }
            OnDequeueNeuron?.Invoke(nextNeuron);
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

        public IEnumerator<BoardNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}