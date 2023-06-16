using System.Threading.Tasks;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
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

        public override MUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.Get(DataProvider.GetModel()) as MUIExplodeNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }

        protected override void Connect(BoardNeuron other) { }
    }
}