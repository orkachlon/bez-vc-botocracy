using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine;

namespace ExternBoardSystem.Events {
    internal static class InnerBoardEvents {
        
        // element events
        public const string OnElementAdded = "InnerBoardOnElementAdded";
        public const string OnElementRemoved = "InnerBoardOnElementRemoved";
        public const string OnElementAddFailed = "InnerBoardOnElementAddFailed";

        // board events
        public const string OnCreateBoard = "InnerBoardOnCreateBoard";
        public const string OnClickTile = "InnerBoardOnClickTile";
        public const string OnRightClickTile = "InnerBoardOnRightClickTile";
    }

    internal class OnElementEventData<TElement> : EventParams 
    where TElement : BoardElement {
        public TElement Element;
        public Vector3Int Cell;

        public OnElementEventData(TElement element, Vector3Int cell) {
            Element = element;
            Cell = cell;
        }
    }

    internal class OnBoardEventData<TElement> : EventParams
    where TElement : BoardElement {
        public IBoard<TElement> Board;
        public IBoardManipulation Manipulator;

        public OnBoardEventData(IBoard<TElement> board, IBoardManipulation boardManipulation) {
            Board = board;
            Manipulator = boardManipulation;
        }
    }

    internal class OnInputEventData : EventParams {
        public Vector3Int Cell;

        public OnInputEventData(Vector3Int cell) {
            Cell = cell;
        }
    }
}