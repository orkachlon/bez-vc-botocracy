using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;

namespace MyHexBoardSystem.BoardSystem.Elements
{
    /// <summary>
    ///     Any kind of elementData which can be positioned onto the board.
    ///     <remarks> Inherit this class to create itens, monsters and stuff to populate the board. </remarks>
    ///     >
    /// </summary>
    public abstract class BoardElement
    {
        protected BoardElement(IElementDataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        public IElementDataProvider DataProvider { get; }
    }

    public interface IElementDataProvider : ExternBoardSystem.BoardSystem.Position.IElementDataProvider {
        
    }
}