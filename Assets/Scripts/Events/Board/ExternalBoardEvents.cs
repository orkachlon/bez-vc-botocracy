﻿using System;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Trait;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Events.Board {
    public static class ExternalBoardEvents {
        
        public const string OnBoardModified = "Board_OnBoardModified";
        public const string OnRemoveTile = "Board_OnRemoveTile";
        public const string OnAddTile = "Board_OnAddTile";
        public const string OnTileOccupied = "Board_OnTileOccupied";
        public const string OnTileUnoccupied = "Board_OnTileUnoccupied";
        public const string OnTileOccupantMoved = "Board_OnTileOccupantMoved";
        public const string OnTileHover = "Board_OnTileHover";

        public const string OnBoardSetupComplete = "Board_OnBoardSetupComplete";
        
        public const string OnBoardBroadCast = "Board_OnBoardBroadCast";

        // trait compass
        public const string OnTraitCompassHide = "Board_OnTraitCompassHide";
        public const string OnTraitCompassEnter = "Board_OnTraitCompassEnter";
        public const string OnTraitCompassExit = "Board_OnTraitCompassExit";
        public const string OnTraitCompassEnterStatic = "Board_OnTraitCompassEnterStatic";
        public const string OnTraitCompassExitStatic = "Board_OnTraitCompassExitStatic";

        // by click
        public const string OnPlaceElementPreActivation = "Board_OnAddElementPreActivation";
        public const string OnPlaceElementTurnDone = "Board_OnPlaceElement";
        public const string OnPlaceElementFailed = "Board_OnPlaceElementFailed";
        
        // by any means
        public const string OnAddElement = "Board_OnAddElement";
        public const string OnRemoveElement = "Board_OnRemoveElement";
        public const string OnMoveElement = "Board_OnMoveElement";
        public const string OnSetFirstElement = "Board_OnSetFirstElement";
        public const string OnSingleNeuronTurn = "Board_OnSingleNeuronTurn";
        public const string OnAllNeuronsDone = "Board_OnAllNeuronsDone";
        public const string OnBoardFull = "Board_OnBoardFull";
        
        
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
        public const string OnAllTraitsOutOfTiles = "Board_OnAllTraitsOutOfTiles";
        
        // stat recording
        public const string OnDummySpawned = "Neurons_OnDummySpawned";
        public const string OnNeuronExploded = "Neurons_OnNeuronExploded";
    }

    public class BoardElementEventArgs<TElement> : EventArgs where TElement : IBoardElement {
        public readonly TElement Element;
        public Hex Hex;

        public BoardElementEventArgs(TElement element, Hex hex) {
            Element = element;
            Hex = hex;
        }
    }

    public class BoardElementMovedEventArgs<TElement> : BoardElementEventArgs<TElement>
        where TElement : IBoardElement {
        public Hex To;
        
        public BoardElementMovedEventArgs(TElement element, Hex from, Hex to) : base(element, from) {
            To = to;
        }
    }

    public class BoardStateEventArgs : EventArgs {
        public readonly IBoardNeuronsController ElementsController;

        public BoardStateEventArgs(IBoardNeuronsController controller) {
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

    public class AllTraitsOutOfTilesEventArgs : EventArgs {
        
    }

    public class TraitCompassHoverEventArgs : EventArgs {
        public ETrait? HighlightedTrait;

        public TraitCompassHoverEventArgs(ETrait? trait) {
            HighlightedTrait = trait;
        }
    }

    public class TileHoverArgs : EventArgs {
        public Hex HexTile;
        public bool LegalPlacement;

        public TileHoverArgs(Hex hexTile, bool legalPlacement) {
            HexTile = hexTile;
            LegalPlacement = legalPlacement;
        }
    }
}