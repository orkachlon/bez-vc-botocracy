using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardSystem.Position;
using Types.Board;
using Types.Hex.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     A board is composed by positions that, by themselves, contain a HexCoordinate.
    ///     Positions may store the game elementData. Things like monsters, items, heroes, etc.
    /// </summary>
    public class Board<T> : IBoard<T>  where T : IBoardElement{
        public EOrientation Orientation { get; }
        public List<IPosition<T>> Positions { get => _positions.Values.ToList(); }

        private ConcurrentDictionary<Hex, IPosition<T>> _positions;

        public Board(IBoardController<T> controller, EOrientation orientation) {
            Orientation = orientation;
            GeneratePositions(controller);
        }

        // public Board(IBoard<T> other) {
        //     Positions = other.Positions.ConvertAll(p => new Position<T>(p.Point, p.Data));
        //     Orientation = other.Orientation;
        // }
        
        public bool HasPosition(Hex point) {
            return GetPosition(point) != null;
        }

        public IPosition<T> GetPosition(Hex point) {
            _positions.TryGetValue(point, out var pos);
            return pos;
        }

        public void RemovePosition(Hex point) {
            var pos = _positions[point];
            if (pos.HasData()) {
                pos.RemoveData();
            }
            _positions.TryRemove(point, out _);
        }

        public void AddPosition(Hex hex) {
            _positions[hex] = new Position<T>(hex);
        }

        private void GeneratePositions(IBoardController<T> hexProvider) {
            var points = hexProvider.GetHexPoints();
            _positions = new ConcurrentDictionary<Hex, IPosition<T>>();
            foreach (var hex in points) {
                _positions[hex] = new Position<T>(hex);
            }

            // OnCreateBoard();
        }

        // private void OnCreateBoard() {
        //     Controller.DispatchCreateBoard(this);
        // }
    }
}