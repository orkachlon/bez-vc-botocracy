using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {
        public ExpandNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding)) {
        }
        
        public override void Activate(SEventManager _, IBoardNeuronsController controller, Vector3Int cell) {
            var neighbours = controller.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                if (!controller.Board.HasPosition(neighbour) || controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                var newElement = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
                controller.AddElement(newElement, neighbour);
            }
        }
    }
}