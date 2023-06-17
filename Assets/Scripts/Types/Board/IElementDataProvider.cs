using UnityEngine;

namespace Types.Board {
    public interface IElementDataProvider<out TElement, out TUIElement> where TElement : IBoardElement {
        TElement GetElement();
        TUIElement GetModel();
        Sprite GetBoardArtwork();
    }
}