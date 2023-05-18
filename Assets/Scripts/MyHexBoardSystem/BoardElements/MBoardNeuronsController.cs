using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using MyHexBoardSystem.BoardElements.Neuron;

namespace MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron> {
        protected override void Awake() {
            base.Awake();
        }

        public override void AddElement(BoardNeuron element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (position.HasData()) {
                print($"Tile {hex} is occupied!");
                return;
            }

            position.AddData(element);
            element.ElementData.GetActivation().Invoke(this, GetCellCoordinate(hex));
            
            // dispatch event
            base.AddElement(element, hex);
        }

        public override void RemoveElement(Hex hex) {
            base.RemoveElement(hex);
        }
    }
}