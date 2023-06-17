using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;

namespace Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {

        private MUIInvulnerableNeuron _uiNeuron;
        
        public InvulnerableNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable)) { }

        public override void Activate() { }
        public override IUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUIInvulnerableNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}