using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {

        private MUIDummyNeuron _uiNeuron;

        public DummyNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy);
        }

        public override INeuronDataBase DataProvider { get; }
        
        public override void Activate() { }
        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}