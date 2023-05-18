using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardSystem {
    public interface IBoardController {
        #region Properties
        public IBoard Board { get; }
        public IBoardManipulation BoardManipulation { get; }
        #endregion

        #region Functions

        // should this be here?
        public void DispatchCreateBoard(IBoard board);
        
        public Hex[] GetHexPoints();
        
        #endregion
    }
}