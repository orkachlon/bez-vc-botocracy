using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {
        public ExplodeNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Exploding)) { }

        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) {
            var neighbours = controller.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                if (!controller.Board.HasPosition(neighbour)) {
                    continue;
                }
                var neighbourPos = controller.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || 
                    ENeuronType.Decaying.Equals(neighbourPos.Data.DataProvider.Type) || 
                    ENeuronType.Invulnerable.Equals(neighbourPos.Data.DataProvider.Type))
                    continue;
                // explode this neuron
                controller.RemoveElement(neighbour);
            }
        }
    }
}