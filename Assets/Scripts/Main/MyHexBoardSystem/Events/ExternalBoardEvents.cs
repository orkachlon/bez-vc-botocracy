﻿using System;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.BoardSystem.Position;
using Main.MyHexBoardSystem.BoardElements;
using Main.Traits;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.MyHexBoardSystem.Events {
    public static class ExternalBoardEvents {
        public const string OnRemoveTile = "Board_OnRemoveTile";
        public const string OnAddTile = "Board_OnAddTile";

        public const string OnBoardSetupComplete = "Board_OnBoardSetupComplete";
        
        public const string OnBoardBroadCast = "Board_OnBoardBroadCast";

        public const string OnBoardEffect = "Board_OnBoardEffect";
        
        // by click
        public const string OnPlaceElement = "Board_OnPlaceElement";
        // by any means
        public const string OnAddElement = "Board_OnAddElement";
        public const string OnRemoveElement = "Board_OnRemoveElement";
        public const string OnSetFirstElement = "Board_OnSetFirstElement";
        
        
        // input events
        public const string OnPointerDown = "Board_OnPointerDown";
        public const string OnPointerUp = "Board_OnPointerUp";
        public const string OnPointerClick = "Board_OnPointerClick";
        public const string OnBeginDrag = "Board_OnBeginDrag";
        public const string OnDrag = "Board_OnDrag";
        public const string OnEndDrag = "Board_OnEndDrag";
        public const string OnDrop = "Board_OnDrop";
        public const string OnPointerEnter = "Board_OnPointerEnter";
        public const string OnPointerExit = "Board_OnPointerExit";
        public const string OnPointerStay = "Board_OnPointerStay";
        public const string OnTraitOutOfTiles = "Board_OnTraitOutOfTiles";
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
        public readonly IBoardNeuronsController ElementsController;

        public OnBoardStateBroadcastEventArgs(IBoardNeuronsController controller) {
            ElementsController = controller;
        }
    }

    public class OnBoardInputEventArgs : EventArgs {
        public PointerEventData EventData;

        public OnBoardInputEventArgs(PointerEventData data) {
            EventData = data;
        }
    }

    public class OnBoardPointerStayEventArgs : EventArgs {
        public Vector2 MousePosition;

        public OnBoardPointerStayEventArgs(Vector2 mousePosition) {
            MousePosition = mousePosition;
        }
    }

    public class OnTileModifyEventArgs : EventArgs {
        public Hex Hex;

        public OnTileModifyEventArgs(Hex hex) {
            Hex = hex;
        }
    }
    
    public class TraitOutOfTilesEventArgs : EventArgs {
        public ETrait Trait;
        public TraitOutOfTilesEventArgs(ETrait trait) {
            Trait = trait;
        }
    }
}