using Main.MyHexBoardSystem.BoardElements.Neuron;

namespace OldGridSystem.Tiles {
    public class Neuron {
        public Neuron(SNeuronData dataProvider) {
            NeuronData = dataProvider;
        }
        
        public SNeuronData NeuronData { get; }
    }
}