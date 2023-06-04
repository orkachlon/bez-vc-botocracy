using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.BoardSystem.Position;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     A board is composed by positions that, by themselves, contain a HexCoordinate.
    ///     Positions may store the game elementData. Things like monsters, items, heroes, etc.
    /// </summary>
    public class Board<T> : IBoard<T>  where T : BoardElement{
        public EOrientation Orientation { get; }
        public List<Position<T>> Positions { get; private set; }

        public Board(IBoardController<T> controller, EOrientation orientation) {
            Orientation = orientation;
            GeneratePositions(controller);
        }

        public Board(IBoard<T> other) {
            Positions = other.Positions.ConvertAll(p => new Position<T>(p.Point, p.Data));
            Orientation = other.Orientation;
        }
        
        public bool HasPosition(Hex point) {
            return GetPosition(point) != null;
        }

        public Position<T> GetPosition(Hex point) {
            return Positions.FirstOrDefault(pos => pos.Point == point);
        }

        public void RemovePosition(Hex point) {
            // this is not optimal
            var pos = Positions.First(p => p.Point == point);
            if (pos.HasData()) {
                pos.RemoveData();
            }
            Positions.Remove(pos);
        }

        public void AddPosition(Hex hex) {
            Positions.Add(new Position<T>(hex));
        }

        private void GeneratePositions(IBoardController<T> hexProvider) {
            var points = hexProvider.GetHexPoints();
            Positions = new List<Position<T>>(points.Length);
            foreach (var hex in points) {
                Positions.Add(new Position<T>(hex));
            }

            // OnCreateBoard();
        }

        // private void OnCreateBoard() {
        //     Controller.DispatchCreateBoard(this);
        // }
    }
}