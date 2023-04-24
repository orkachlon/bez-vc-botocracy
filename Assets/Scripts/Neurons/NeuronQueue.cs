using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neurons {
    public class NeuronQueue : MonoBehaviour {

        [SerializeField] private float neuronSpacing;
        
        private Queue<Neuron> _neurons;

        private void Awake() {
            _neurons = new Queue<Neuron>();
        }

        public void EnqueueAll(IEnumerable<Neuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(Neuron neuron) {
            neuron.transform.position = transform.position + Vector3.right * _neurons.Count * neuronSpacing;
            _neurons.Enqueue(neuron);
        }
        
        public Neuron Dequeue() {
            foreach (var neuron in _neurons) {
                neuron.transform.position += Vector3.left * neuronSpacing;
            }
            return _neurons.Dequeue();
        }
    }
}