using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {
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

        protected override void Connect(BoardNeuron other) { }
    }
}