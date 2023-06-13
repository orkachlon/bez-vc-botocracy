using Core.Utils;
using Main.Neurons.Data;
using Main.Neurons.Runtime;

namespace Main.Neurons {
    public class Neuron : BoardNeuron {
        public Neuron(SNeuronDataBase dataProvider) : base(dataProvider) { }
        
        public Neuron(BoardNeuron boardNeuron) : base(boardNeuron.DataProvider) { }

        public ENeuronUIState UIState { get; set; }
        
        
        public override void Activate() {
            MLogger.LogEditor("Tried to activate UI neuron!!!");
        }
    }
    
    public enum ENeuronUIState {
        First = 0,
        Second = 1,
        Third = 2,
        Stack = 3
    }
}