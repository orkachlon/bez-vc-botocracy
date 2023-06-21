using System.Threading.Tasks;
using Types.Neuron.Runtime;

namespace Types.Neuron.Connections {
    public interface IConnectionManager {
        Task Connect(IBoardNeuron n1, IBoardNeuron n2);
        Task Disconnect(IBoardNeuron n1, IBoardNeuron n2);
    }
}