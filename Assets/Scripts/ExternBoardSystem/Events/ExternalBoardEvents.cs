using System;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.Events {
    public static class ExternalBoardEvents {
        // by click
        public const string OnPlaceElement = "BoardOnPlaceElement";

        public const string OnBoardSetupComplete = "BoardOnBoardSetupComplete";
        
        // by any means
        public const string OnAddElement = "BoardOnAddElement";
        public const string OnRemoveElement = "BoardOnRemoveElement";
        public const string OnSetFirstElement = "BoardOnSetFirstElement";
    }

    public class OnPlaceElementEventArgs<TElement> : EventArgs where TElement : BoardElement {
        public readonly TElement Element;
        public Hex Hex;

        public OnPlaceElementEventArgs(TElement element, Hex hex) {
            Element = element;
            Hex = hex;
        }
    }
}