using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     A board is composed by positions that, by themselves, contain a HexCoordinate.
    ///     Positions may store the game elementData. Things like monsters, items, heroes, etc.
    /// </summary>
    public class Board<T> : IBoard<T>  where T : BoardElement{
        public IBoardController<T> Controller { get; }
        public EOrientation Orientation { get; }
        public Position.Position<T>[] Positions { get; private set; }

        public Board(IBoardController<T> controller, EOrientation orientation) {
            Controller = controller;
            Orientation = orientation;
            GeneratePositions();
        }
        
        public bool HasPosition(Hex point) {
            return GetPosition(point) != null;
        }

        public Position.Position<T> GetPosition(Hex point) {
            foreach (var i in Positions)
                if (i.Point == point)
                    return i;

            return null;
        }
        
        private void GeneratePositions() {
            var points = Controller.GetHexPoints();
            Positions = new Position.Position<T>[points.Length];
            for (var index = 0; index < points.Length; index++) {
                var i = points[index];
                Positions[index] = new Position.Position<T>(i);
            }

            // OnCreateBoard();
        }
        
        // private void OnCreateBoard() {
        //     Controller.DispatchCreateBoard(this);
        // }
    }
}