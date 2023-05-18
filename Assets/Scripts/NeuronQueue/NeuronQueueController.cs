using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Managers;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class NeuronQueueController : MonoBehaviour, IEnumerable<MNeuron> {

        public int Count => _neurons.Count;
        
        public event Action<MNeuron> OnEnqueueNeuron;
        public event Action<MNeuron> OnDequeueNeuron;

        private Queue<MNeuron> _neurons;

        private void Awake() {
            _neurons = new Queue<MNeuron>();
        }

        public void Enqueue(IEnumerable<MNeuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(MNeuron neuron) {
            _neurons.Enqueue(neuron);
            OnEnqueueNeuron?.Invoke(neuron);
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                Enqueue(Instantiate(NeuronManager.Instance.GetRandomNeuron(), transform.position, Quaternion.identity, transform));
            }
        }
        
        public MNeuron Dequeue() {
            if (_neurons.Count == 0) {
                return null;
            }
            var nextNeuron = _neurons.Dequeue();
            OnDequeueNeuron?.Invoke(nextNeuron);
            return nextNeuron;
        }

        public MNeuron Peek() {
            return _neurons.Peek();
        }

        public MNeuron PeekLast() {
            return _neurons.ToArray()[_neurons.Count - 1];
        }

        [CanBeNull]
        public MNeuron Peek(int index) {
            if (0 <= index && index < _neurons.Count) {
                return _neurons.ToArray()[index];
            }

            return null;
        }

        public IEnumerator<MNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}