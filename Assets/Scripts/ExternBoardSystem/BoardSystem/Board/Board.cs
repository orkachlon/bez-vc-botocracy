using ExternBoardSystem.BoardSystem.BoardShape;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     A board is composed by positions that, by themselves, contain a HexCoordinate.
    ///     Positions may store the game elementData. Things like monsters, items, heroes, etc.
    /// </summary>
    public class Board : IBoard {
        public BoardController Controller { get; }
        public Orientation Orientation { get; }
        public Position.Position[] Positions { get; private set; }

        public Board(BoardController controller, Orientation orientation) {
            Controller = controller;
            Orientation = orientation;
            GeneratePositions();
        }
        
        public bool HasPosition(Hex point) {
            return GetPosition(point) != null;
        }

        public Position.Position GetPosition(Hex point) {
            foreach (var i in Positions)
                if (i.Point == point)
                    return i;

            return null;
        }
        
        private void GeneratePositions() {
            var points = Controller.GetHexPoints();
            Positions = new Position.Position[points.Length];
            for (var index = 0; index < points.Length; index++) {
                var i = points[index];
                Positions[index] = new Position.Position(i);
            }

            OnCreateBoard();
        }
        
        private void OnCreateBoard() {
            Controller.DispatchCreateBoard(this);
        }
    }
}