using System.Threading.Tasks;
using Events.Board;
using Events.Neuron;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {

        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }
        protected MUIInvulnerableNeuron UIInvulNeuron => UINeuron as MUIInvulnerableNeuron; 
        
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

        public override async Task AwaitAddition() {
            UIInvulNeuron.PlayAddSound();
            UIInvulNeuron.PlayAddAnimation();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }
    }
}