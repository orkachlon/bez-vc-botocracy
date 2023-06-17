using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Types.Board.UI;
using Types.Neuron;

namespace Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {

        private MUIExpandNeuron _uiNeuron;
        
        public ExpandNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding)) { }
        
        public override void Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour) || Controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                var newElement = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
                Controller.AddElement(newElement, neighbour);
            }
        }

        public override IUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUIExpandNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }
    }
}