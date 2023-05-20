using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.Events {
    public static class ExternalBoardEvents {
        // by click
        public const string OnPlaceElement = "BoardOnPlaceElement";
        
        public const string OnSetFirstElement = "BoardOnSetFirstElement";
        
        // by any means
        public const string OnAddElement = "BoardOnAddElement";
        public const string OnRemoveElement = "BoardOnRemoveElement";
    }

    public class OnPlaceElementData<TElement> : EventParams where TElement : BoardElement {
        public readonly TElement Element;
        public Hex Hex;

        public OnPlaceElementData(TElement element, Hex hex) {
            Element = element;
            Hex = hex;
        }
    }
}