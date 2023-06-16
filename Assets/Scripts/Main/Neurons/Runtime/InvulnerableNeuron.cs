using System.Threading.Tasks;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {

        private MUIInvulnerableNeuron _uiNeuron;
        
        public InvulnerableNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable)) { }

        public override void Activate() { }
        public override MUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.Get(DataProvider.GetModel()) as MUIInvulnerableNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}