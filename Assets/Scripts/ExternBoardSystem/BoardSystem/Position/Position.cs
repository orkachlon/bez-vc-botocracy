using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem.Position
{
    /// <summary>
    ///     A position in a real game most likely stores some sort of elementData.
    ///     Things like heroes, buildings, monsters, items, etc. Be creative.
    ///     <remarks> If this structure grow consider make it a class and pool it, instead. </remarks>
    ///     >
    /// </summary>
    public class Position <T> where T : BoardElement {
        public Position(Hex point, T baseData = null) {
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
            Data = null;
        }

        public bool HasData() {
            return Data != null;
        }
    }
}