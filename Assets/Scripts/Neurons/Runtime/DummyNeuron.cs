using System.Threading.Tasks;
using Animation;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {
        
        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }


        public DummyNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy);
            Connector = NeuronFactory.GetConnector();
        }

        public override async Task Activate() {
            await AnimationManager.WaitForElement(this);
            ReportTurnDone();
        }
        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }
    }
}