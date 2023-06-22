using System.Threading.Tasks;
using Types.Neuron.Connections;
using Types.Neuron.Runtime;

namespace Neurons.Connections {
    public class BoardNeuronConnector : IBoardNeuronConnector {
        public IConnectionManager ConnectionManager { get; }

        public BoardNeuronConnector(IConnectionManager connectionManager) {
            ConnectionManager = connectionManager;
        }


        public async Task Connect(IBoardNeuron n1, IBoardNeuron n2, int delay = 0) {
            await Task.Delay(delay);
            await ConnectionManager.Connect(n1, n2);
        }

        public async Task Disconnect(IBoardNeuron n1, IBoardNeuron n2, int delay = 0) {
            await Task.Delay(delay);
            await ConnectionManager.Disconnect(n1, n2);
        }
    }
}