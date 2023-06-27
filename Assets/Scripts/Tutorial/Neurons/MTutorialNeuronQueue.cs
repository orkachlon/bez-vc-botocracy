using Core.EventSystem;
using Events.Board;
using Events.Neuron;
using Events.Tutorial;
using Neurons.NeuronQueue;
using System;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Tutorial.Neurons {
    public class MTutorialNeuronQueue : MNeuronQueue {

        [SerializeField] private SEventManager tutorialEventManager;

        protected override void OnEnable() {
            tutorialEventManager.Register(TutorialEvents.OnEnableBoard, StartProvidingNeurons);
            tutorialEventManager.Register(TutorialEvents.OnDisableBoard, StopProvidingNeurons);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementPreActivation, OnNeuronPlaced);
        }

        protected override void OnDisable() {
            tutorialEventManager.Unregister(TutorialEvents.OnEnableBoard, StartProvidingNeurons);
            tutorialEventManager.Unregister(TutorialEvents.OnDisableBoard, StopProvidingNeurons);
            neuronEventManager.Unregister(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementPreActivation, OnNeuronPlaced);
        }

        public override IStackNeuron Dequeue() {
            // we dequeue the neuron that was placed just now.
            // the next neuron is the one after the dequeued one
            Neurons.TryDequeue(out var prevNeuron);
            if (prevNeuron == null) {
                StopProvidingNeurons(); // shouldn't happen, just to be sure
                return null;
            }
            // never run out of neurons but keep visibility and functionality the same
            if (IsInfinite) {
                Enqueue(1);
            }

            neuronEventManager.Raise(NeuronEvents.OnDequeueNeuron, new NeuronQueueEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));

            if (Neurons.Count > 0) {
                return prevNeuron;
            }
            StopProvidingNeurons();
            tutorialEventManager.Raise(TutorialEvents.OnQueueDepleted, new NeuronQueueEventArgs(this));
            return null;
        }

        private void Hide(EventArgs args = null) {

        }
    }
}