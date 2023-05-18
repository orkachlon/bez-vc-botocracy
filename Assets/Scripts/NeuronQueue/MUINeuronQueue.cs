using System;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {
        [SerializeField] private NeuronQueueController controller;
        [SerializeField] private float neuronSpacing;
        [SerializeField] private int neuronsToShow = 5;

        private void Awake() {
            controller.OnEnqueueNeuron += OnEnqueue;
            controller.OnDequeueNeuron += OnDequeue;
        }


        private void OnDestroy() {
            controller.OnEnqueueNeuron -= OnEnqueue;
            controller.OnDequeueNeuron -= OnDequeue;
        }

        private void OnEnqueue(MNeuron neuron) {
            if (controller.Count > neuronsToShow) {
                neuron.Hide();
            }
            neuron.transform.position = transform.position + Vector3.right * controller.Count * neuronSpacing;
        }
        
        private void OnDequeue(MNeuron neuron) {
            foreach (var n in controller) {
                n.transform.position += Vector3.left * neuronSpacing;
            }
            if (controller.Count >= neuronsToShow) {
                controller.Peek(neuronsToShow - 1)?.Show();
            }
        }
    }
}