using Core.EventSystem;
using MyHexBoardSystem.BoardElements.Neuron;

namespace Neurons {
    public static class NeuronEvents {
        public const string OnNeuronPlaced = "NeuronsOnNeuronPlaced";
        public const string OnNoMoreNeurons = "NeuronsOnNoMoreNeurons";
        
        // queue events
        public const string OnEnqueueNeuron = "NeuronsOnEnqueueNeuron";
        public const string OnDequeueNeuron = "NeuronsOnDequeueNeuron";
        public const string OnRequestNeuronFromQueue = "NeuronsOnRequestNeuronFromQueue";
    }

    public class NeuronEvent : EventParams {
        public BoardNeuron Neuron;

        public NeuronEvent(BoardNeuron neuron) {
            Neuron = neuron;
        }
        
        public NeuronEvent(ref BoardNeuron neuron) {
            Neuron = neuron;
        }
    }
    
    public class NeuronDataEvent : EventParams {
        public SNeuronData NeuronData;

        public NeuronDataEvent(ref SNeuronData neuronData) {
            NeuronData = neuronData;
        }
    }
}