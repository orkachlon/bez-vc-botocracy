using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;

namespace Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {

        private MUIDummyNeuron _uiNeuron;
        
        public DummyNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy)) { }

        public override void Activate() { }
        public override IUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUIDummyNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}