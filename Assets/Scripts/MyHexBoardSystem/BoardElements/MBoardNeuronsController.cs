using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using MyHexBoardSystem.BoardElements.Neuron;

namespace MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron> {
        public override void AddElement(BoardNeuron element, Hex hex) {
            base.AddElement(element, hex);
        }

        public override void RemoveElement(Hex hex) {
            throw new System.NotImplementedException();
        }
    }
}