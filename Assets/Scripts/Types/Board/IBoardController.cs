
namespace Types.Board {
    public interface IBoardController<T> where T : IBoardElement {
        #region Properties
        public IBoard<T> Board { get; }
        public IBoardManipulation Manipulator { get; }
        #endregion

        #region Functions
        
        public Hex.Coordinates.Hex[] GetHexPoints();
        
        #endregion
    }
}