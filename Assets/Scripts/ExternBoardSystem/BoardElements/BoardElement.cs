using Types.Board;
using Types.Board.UI;

namespace ExternBoardSystem.BoardElements
{
    /// <summary>
    ///     Any kind of elementData which can be positioned onto the board.
    ///     <remarks> Inherit this class to create items, monsters and stuff to populate the board. </remarks>
    /// </summary>
    public abstract class BoardElement : IBoardElement {
        protected BoardElement(IElementDataProvider<IBoardElement, IUIBoardElement> dataProvider) {
            DataProvider = dataProvider;
        }

        public IElementDataProvider<IBoardElement, IUIBoardElement> DataProvider { get; }

        public override string ToString() {
            return DataProvider.ToString();
        }
    }
}