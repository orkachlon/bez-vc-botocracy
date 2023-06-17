using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {

        private MUIExplodeNeuron _uiNeuron;
        
        public ExplodeNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Exploding)) {
            Connectable = false;
        }

        public override void Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour)) {
                    continue;
                }
                var neighbourPos = Controller.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || 
                    ENeuronType.Decaying.Equals(neighbourPos.Data.DataProvider.Type) || 
                    ENeuronType.Invulnerable.Equals(neighbourPos.Data.DataProvider.Type))
                    continue;
                // explode this neuron
                Controller.RemoveNeuron(neighbour);
            }
        }

        public override IUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUIExplodeNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }

        public override void Connect() { }
    }
}