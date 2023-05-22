using System;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;

namespace Main.MyHexBoardSystem.BoardSystem {
    public static class ExternalBoardEvents {

        public const string OnBoardSetupComplete = "BoardOnBoardSetupComplete";
        
        public const string OnBoardBroadCast = "BoardOnBoardBroadCast";
        
        // by click
        public const string OnPlaceElement = "BoardOnPlaceElement";
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

    public class OnBoardStateBroadcastEventArgs : EventArgs {
        public readonly IBoardNeuronController ElementsController;

        public OnBoardStateBroadcastEventArgs(IBoardNeuronController controller) {
            ElementsController = controller;
        }
    }
}