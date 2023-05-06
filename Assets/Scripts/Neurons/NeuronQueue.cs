using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Neurons {
    public class NeuronQueue : MonoBehaviour {

        [SerializeField] private float neuronSpacing;
        [SerializeField] private int neuronsToShow = 5;
        
        private Queue<Neuron> _neurons;

        private void Awake() {
            _neurons = new Queue<Neuron>();
        }

        public void Enqueue(IEnumerable<Neuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }
        
        public void Enqueue(Neuron neuron) {
            if (_neurons.Count > neuronsToShow) {
                neuron.Hide();
            }
            neuron.transform.position = transform.position + Vector3.right * _neurons.Count * neuronSpacing;
            _neurons.Enqueue(neuron);
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                Enqueue(Instantiate(NeuronManager.Instance.GetRandomNeuronPrefab(), transform.position, Quaternion.identity, transform));
            }
        }
        
        public Neuron Dequeue() {
            if (_neurons.Count == 0) {
                return null;
            }
            foreach (var neuron in _neurons) {
                neuron.transform.position += Vector3.left * neuronSpacing;
            }

            var nextNeuron = _neurons.Dequeue();
            if (_neurons.Count >= neuronsToShow) {
                _neurons.ToArray()[neuronsToShow - 1].Show();
            }
            return nextNeuron;
        }
    }
}