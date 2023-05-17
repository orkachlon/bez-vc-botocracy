using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     Interface of a runtime board. It has a shape with its Hex points, orientation and positions to store elementData.
    /// </summary>
    public interface IBoard {
        BoardController Controller { get; }
        Orientation Orientation { get; }
        Position.Position[] Positions { get; }
        bool HasPosition(Hex point);
        Position.Position GetPosition(Hex point);
    }
}