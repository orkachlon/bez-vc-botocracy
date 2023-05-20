using Core.EventSystem;
using MyHexBoardSystem.BoardElements.Neuron;

namespace Neurons {
    public static class NeuronEvents {
        public const string OnNeuronPlaced = "NeuronsOnNeuronPlaced";
        public const string OnNoMoreNeurons = "NeuronsOnNoMoreNeurons";
        
        // queue events
        public const string OnEnqueueNeuron = "NeuronsOnEnqueueNeuron";
        public const string OnDequeueNeuron = "NeuronsOnDequeueNeuron";
    }

    public class NeuronEvent : EventParams {
        public readonly BoardNeuron Neuron;

        public NeuronEvent(BoardNeuron neuron) {
            Neuron = neuron;
        }
        
        public NeuronEvent() {
            Neuron = null;
        }
    }
}