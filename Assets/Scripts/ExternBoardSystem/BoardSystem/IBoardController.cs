using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem {
    public interface IBoardController<T> where T : BoardElement {
        #region Properties
        public IBoard<T> Board { get; }
        public IBoardManipulation Manipulator { get; }
        #endregion

        #region Functions
        
        public Hex[] GetHexPoints();
        
        #endregion
    }
}