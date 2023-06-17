using Types.Neuron.Data;
using Types.Neuron.UI;

namespace Types.Neuron.Runtime {
    public interface IStackNeuron {
        public IBoardNeuron BoardNeuron { get; }
        public INeuronDataBase DataProvider { get; }
        ENeuronUIState UIState { get; set; }
    }
}