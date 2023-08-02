using System;
using Types.Board;
using UnityEngine;

namespace ExternBoardSystem.Events {
    internal static class InnerBoardEvents {
        public const string OnElementMoved = "InnerBoard_OnElementMoved";

        // element events
        public const string OnElementAdded = "InnerBoard_OnElementAdded";
        public const string OnElementRemoved = "InnerBoard_OnElementRemoved";
        public const string OnElementAddFailed = "InnerBoard_OnElementAddFailed";

        // board events
        public const string OnCreateBoard = "InnerBoard_OnCreateBoard";
        public const string OnClickTile = "InnerBoard_OnClickTile";
        public const string OnRightClickTile = "InnerBoard_OnRightClickTile";
        public const string OnEnterTile = "InnerBoard_OnEnterTile";
    }

    internal class OnElementEventData<TElement> : EventArgs 
    where TElement : IBoardElement {
        public TElement Element;
        public Vector3Int Cell;

        public OnElementEventData(TElement element, Vector3Int cell) {
            Element = element;
            Cell = cell;
        }
    }

    internal class OnElementMovedEventData<TElement> : OnElementEventData<TElement> 
        where TElement : IBoardElement {
        public Vector3Int ToCell;

        public OnElementMovedEventData(TElement element, Vector3Int fromCell, Vector3Int toCell) : base(element, fromCell) {
            ToCell = toCell;
        }
    }

    internal class OnBoardEventData<TElement> : EventArgs
    where TElement : IBoardElement {
        public IBoard<TElement> Board;
        public IBoardManipulation Manipulator;

        public OnBoardEventData(IBoard<TElement> board, IBoardManipulation boardManipulation) {
            Board = board;
            Manipulator = boardManipulation;
        }
    }

    internal class InputEventData : EventArgs {
        public Vector3Int Cell;

        public InputEventData(Vector3Int cell) {
            Cell = cell;
        }
    }
    
}