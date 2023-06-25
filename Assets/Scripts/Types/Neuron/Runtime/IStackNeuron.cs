using System.Threading.Tasks;
using Types.Neuron.Data;
using Types.Neuron.UI;

namespace Types.Neuron.Runtime {
    public interface IStackNeuron {
        public IBoardNeuron BoardNeuron { get; }
        public INeuronDataBase DataProvider { get; }
        int PlaceInQueue { get; set; }

        public Task PlayStackAnimation();
    }
}