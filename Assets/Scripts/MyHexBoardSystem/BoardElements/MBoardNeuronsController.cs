using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using MyHexBoardSystem.BoardElements.Neuron;

namespace MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron> {

        public override void AddElement(BoardNeuron element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (position.HasData()) {
                print($"Tile {hex} is occupied!");
                return;
            }

            var cell = GetCellCoordinate(hex);
            // check if any neighbors exist
            var neighbours = Manipulator.GetNeighbours(cell);
            var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) && 
                                                           Board.GetPosition(neighbour).HasData());
            if (!hasNeighbour) {
                return;
            }

            position.AddData(element);
            element.ElementData.GetActivation().Invoke(this, GetCellCoordinate(hex));
            
            // dispatch event
            DispatchOnAddElement(element, cell);
        }

        public override void RemoveElement(Hex hex) {
            base.RemoveElement(hex);
        }
    }
}