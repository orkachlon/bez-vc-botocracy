using System.Threading.Tasks;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {

        private MUIDummyNeuron _uiNeuron;
        
        public DummyNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy)) { }

        public override void Activate() { }
        public override MUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.Get(DataProvider.GetModel()) as MUIDummyNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}