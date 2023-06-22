using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {

        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }
        
        public InvulnerableNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable);
            Connector = NeuronFactory.GetConnector();
        }
        
        public override Task Activate() {
            ReportTurnDone();
            return Task.CompletedTask;
        }
        
        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }
    }
}