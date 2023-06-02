using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     Interface of a runtime board. It has a shape with its Hex points, orientation and positions to store elementData.
    /// </summary>
    public interface IBoard<T> where T : BoardElement {
        // MBoardController Controller { get; }
        EOrientation Orientation { get; }
        List<Position.Position<T>> Positions { get; }
        bool HasPosition(Hex point);
        Position.Position<T> GetPosition(Hex point);
        void RemovePosition(Hex point);
        void AddPosition(Hex hex);
    }
}