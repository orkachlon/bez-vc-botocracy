using ExternBoardSystem.Ui.Board;
using UnityEngine;

namespace ExternBoardSystem.BoardElements
{
    /// <summary>
    ///     Any kind of elementData which can be positioned onto the board.
    ///     <remarks> Inherit this class to create items, monsters and stuff to populate the board. </remarks>
    /// </summary>
    public abstract class BoardElement {
        protected BoardElement(IElementDataProvider<BoardElement, MUIBoardElement> dataProvider) {
            DataProvider = dataProvider;
        }

        public IElementDataProvider<BoardElement, MUIBoardElement> DataProvider { get; }
    }

    public interface IElementDataProvider<out TElement, out TUIElement> where TElement : BoardElement {
        TElement GetElement();
        TUIElement GetModel();
        Sprite GetArtwork();
    }
}