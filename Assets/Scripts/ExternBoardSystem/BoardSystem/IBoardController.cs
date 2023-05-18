using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem {
    public interface IBoardController<T> where T : BoardElement {
        #region Properties
        public IBoard<T> Board { get; }
        public IBoardManipulation BoardManipulation { get; }
        #endregion

        #region Functions

        // should this be here?
        public void DispatchCreateBoard(IBoard<T> board);
        
        public Hex[] GetHexPoints();
        
        #endregion
    }
}