using System.Threading;
using System.Threading.Tasks;
using Types.Neuron.Runtime;

namespace Types.Neuron.Connections {
    public interface IConnectionManager {
        
        SemaphoreSlim ConnectionLock { get; }
        
        Task Connect(IBoardNeuron n1, IBoardNeuron n2);
        Task Disconnect(IBoardNeuron n1, IBoardNeuron n2);
    }
}