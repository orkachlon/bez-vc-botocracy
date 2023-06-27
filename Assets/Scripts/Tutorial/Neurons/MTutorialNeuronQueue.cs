using Core.EventSystem;
using Events.Neuron;
using Events.Tutorial;
using Neurons.NeuronQueue;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Tutorial.Neurons {
    public class MTutorialNeuronQueue : MNeuronQueue {

        [SerializeField] private SEventManager tutorialEventManager;
        
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
            // neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronQueueEventArgs(this));
            return null;
        }
    }
}