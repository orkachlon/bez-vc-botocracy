using Types.Neuron.Data;

namespace OldGridSystem.Tiles {
    public class Neuron {
        public Neuron(INeuronDataBase dataProvider) {
            NeuronData = dataProvider;
        }
        
        public INeuronDataBase NeuronData { get; }
    }
}