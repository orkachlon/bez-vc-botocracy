using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {
        public ExpandNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding)) {
        }
        
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
    }
}