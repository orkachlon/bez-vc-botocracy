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
    public class InvulnerableNeuron : BoardNeuron {

        public override INeuronDataBase DataProvider { get; }

        public InvulnerableNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable);
        }

        public override Task Activate() => Task.CompletedTask;
        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitRemoval() {
            await UINeuron.PlayRemoveAnimation();
        }
    }
}