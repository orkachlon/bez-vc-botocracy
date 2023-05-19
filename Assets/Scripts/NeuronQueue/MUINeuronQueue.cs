using System.Collections.Generic;
using ExternBoardSystem.Tools;
using MyHexBoardSystem.BoardElements.Neuron;
using MyHexBoardSystem.UI;
using Neurons;
using UnityEngine;

namespace NeuronQueue {
    public class MUINeuronQueue : MonoBehaviour {
        [SerializeField] private NeuronQueueController controller;
        [SerializeField] private float neuronSpacing;
        [SerializeField] private int neuronsToShow = 5;

        private readonly Dictionary<BoardNeuron, MUIBoardNeuron> _registerUiElements = new();

        private void Awake() {
            controller.OnEnqueueNeuron += OnEnqueue;
            controller.OnDequeueNeuron += OnDequeue;
        }


        private void OnDestroy() {
            controller.OnEnqueueNeuron -= OnEnqueue;
            controller.OnDequeueNeuron -= OnDequeue;
        }

        private void OnEnqueue(BoardNeuron neuron) {
            if (_registerUiElements.Count >= neuronsToShow) {
                return;
            }
            ShowNeuron(neuron);
        }
        
        private void OnDequeue(BoardNeuron neuron) {
            // release the dequeued neuron ?
            MObjectPooler.Instance.Release(_registerUiElements[neuron].gameObject);
            _registerUiElements.Remove(neuron);

            // shift neurons
            foreach (var n in _registerUiElements.Values) {
                n.transform.position += Vector3.left * neuronSpacing;
            }
            
            // show the next neuron in queue
            if (_registerUiElements.Count >= neuronsToShow)
                return;
            
            var lastNeuron = controller.Peek(neuronsToShow - 1);
            
            // we have less than 'neuronsToShow' neurons
            if (lastNeuron == null) {
                return;
            }
            
            ShowNeuron(lastNeuron);
        }

        private void ShowNeuron(BoardNeuron neuron) {
            var uiElement = MObjectPooler.Instance.Get<MUIBoardNeuron>(neuron.ElementData.GetModel().gameObject);
            uiElement.SetRuntimeElementData(neuron);
            uiElement.SetWorldPosition(transform.position + Vector3.right * (_registerUiElements.Count * neuronSpacing));
            _registerUiElements.Add(neuron, uiElement);
        }
    }
}