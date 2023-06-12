using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons;
using Main.Neurons.Data;

namespace OldGridSystem.Tiles {
    public class Neuron {
        public Neuron(SNeuronDataBase dataProvider) {
            NeuronData = dataProvider;
        }
        
        public SNeuronDataBase NeuronData { get; }
    }
}