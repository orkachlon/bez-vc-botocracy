using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.BoardSystem.Position;

namespace MyHexBoardSystem.BoardSystem.Board {
    public interface IMyBoard {
        MyBoardController Controller { get; }
        Orientation Orientation { get; }
        Position[] Positions { get; }
        bool HasPosition(Hex point);
        Position GetPosition(Hex point);
    }
}