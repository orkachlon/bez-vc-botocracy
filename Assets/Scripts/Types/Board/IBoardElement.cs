using Types.Board.UI;

namespace Types.Board {
    public interface IBoardElement {
        public IElementDataProvider<IBoardElement, IUIBoardElement> DataProvider { get; }
    }
}