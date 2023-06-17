using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Types.Neuron.UI;

namespace Neurons.Runtime {
    public class StackNeuron : IStackNeuron {

        public IBoardNeuron BoardNeuron { get; }

        public INeuronDataBase DataProvider => BoardNeuron.DataProvider;
        public ENeuronUIState UIState { get; set; }

        public StackNeuron(IBoardNeuron boardNeuron) {
            BoardNeuron = boardNeuron;
        }
    }
}