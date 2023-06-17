using Types.Board;
using Types.Hex.Coordinates;

namespace ExternBoardSystem.BoardSystem.Position
{
    /// <summary>
    ///     A position in a real game most likely stores some sort of elementData.
    ///     Things like heroes, buildings, monsters, items, etc. Be creative.
    ///     <remarks> If this structure grow consider make it a class and pool it, instead. </remarks>
    ///     >
    /// </summary>
    public class Position <T> : IPosition<T> 
        where T : IBoardElement {
        public Position(Hex point, T baseData = default) {
            Point = point;
            Data = baseData;
        }

        /// <summary>
        ///     The elementData in the board.
        ///     <remarks> Consider make it an Array if it can hold more than one single object. </remarks>>
        /// </summary>
        public T Data { get; private set; }

        public Hex Point { get; }

        public void AddData(T baseData) {
            Data = baseData;
        }

        public void RemoveData() {
            Data = default;
        }

        public bool HasData() {
            return Data != null;
        }
    }
}