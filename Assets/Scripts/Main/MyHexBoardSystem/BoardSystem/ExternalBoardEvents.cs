using System;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;

namespace Main.MyHexBoardSystem.BoardSystem {
    public static class ExternalBoardEvents {

        public const string OnBoardSetupComplete = "Board_OnBoardSetupComplete";
        
        public const string OnBoardBroadCast = "Board_OnBoardBroadCast";

        public const string OnBoardEffect = "Board_OnBoardEffect";
        
        // by click
        public const string OnPlaceElement = "Board_OnPlaceElement";
        // by any means
        public const string OnAddElement = "Board_OnAddElement";
        public const string OnRemoveElement = "Board_OnRemoveElement";
        public const string OnSetFirstElement = "Board_OnSetFirstElement";
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