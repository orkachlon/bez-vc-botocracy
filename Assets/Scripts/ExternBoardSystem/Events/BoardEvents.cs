using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.Events {
    public static class BoardEvents {
        public const string OnPlaceElement = "BoardOnPlaceElement";
        public const string OnSetFirstNeuron = "BoardOnSetFirstNeuron";
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