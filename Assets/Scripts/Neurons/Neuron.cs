using MyHexBoardSystem.BoardElements.Neuron;

namespace Neurons {
    public class Neuron {
        public Neuron(SNeuronData dataProvider) {
            NeuronData = dataProvider;
        }
        
        public SNeuronData NeuronData { get; }
    }

    public enum ENeuronType {
        Undefined,
        Invulnerable,
        Exploding,
        Expanding,
        Dummy
    }
}