using System.Collections.Generic;

namespace Types.Board {
    /// <summary>
    ///     Interface of a runtime board. It has a shape with its Hex points, orientation and positions to store elementData.
    /// </summary>
    public interface IBoard<T> where T : IBoardElement {
        // MBoardController Controller { get; }
        EOrientation Orientation { get; }
        List<IPosition<T>> Positions { get; }
        bool HasPosition(Hex.Coordinates.Hex point);
        IPosition<T> GetPosition(Hex.Coordinates.Hex point);
        void RemovePosition(Hex.Coordinates.Hex point);
        void AddPosition(Hex.Coordinates.Hex hex);
    }
}