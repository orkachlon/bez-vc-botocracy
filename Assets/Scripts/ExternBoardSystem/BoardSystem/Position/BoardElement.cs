using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Position
{
    /// <summary>
    ///     Any kind of elementData which can be positioned onto the board.
    ///     <remarks> Inherit this class to create items, monsters and stuff to populate the board. </remarks>
    /// </summary>
    public abstract class BoardElement {
        protected BoardElement(IElementDataProvider dataProvider) {
            DataProvider = dataProvider;
        }

        public IElementDataProvider DataProvider { get; }
    }

    public interface IElementDataProvider {
        BoardElement GetElement();
        GameObject GetModel();
        Sprite GetArtwork();
    }
}