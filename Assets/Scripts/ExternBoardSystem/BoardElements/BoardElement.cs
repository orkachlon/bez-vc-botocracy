using UnityEngine;

namespace ExternBoardSystem.BoardElements
{
    /// <summary>
    ///     Any kind of elementData which can be positioned onto the board.
    ///     <remarks> Inherit this class to create items, monsters and stuff to populate the board. </remarks>
    /// </summary>
    public abstract class BoardElement {
        protected BoardElement(IElementDataProvider<BoardElement> dataProvider) {
            DataProvider = dataProvider;
        }

        public IElementDataProvider<BoardElement> DataProvider { get; }
    }

    public interface IElementDataProvider<out T> where T : BoardElement {
        T GetElement();
        GameObject GetModel();
        Sprite GetArtwork();
    }
}