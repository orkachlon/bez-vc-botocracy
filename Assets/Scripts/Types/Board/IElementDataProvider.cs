using UnityEngine;

namespace Types.Board {
    public interface IElementDataProvider<out TElement, out TUIElement> where TElement : IBoardElement {
        TElement GetNewElement();
        TUIElement GetModel();
        Sprite GetBoardArtwork();
        AudioClip GetAddSound();
        AudioClip GetRemoveSound();
    }
}