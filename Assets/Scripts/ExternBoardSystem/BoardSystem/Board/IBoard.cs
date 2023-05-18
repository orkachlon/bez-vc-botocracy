using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     Interface of a runtime board. It has a shape with its Hex points, orientation and positions to store elementData.
    /// </summary>
    public interface IBoard {
        // MBoardController Controller { get; }
        EOrientation Orientation { get; }
        Position.Position[] Positions { get; }
        bool HasPosition(Hex point);
        Position.Position GetPosition(Hex point);
    }
}