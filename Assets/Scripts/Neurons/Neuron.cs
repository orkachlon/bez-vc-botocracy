using MyHexBoardSystem.BoardElements.Neuron;

namespace Neurons {
    public class Neuron {

        public enum ENeuronType {
            Undefined,
            Invulnerable,
            Exploding,
            Expanding
        }

        public Neuron(SNeuronData dataProvider) {
            NeuronData = dataProvider;
        }
        
        public SNeuronData NeuronData { get; }
    }
}