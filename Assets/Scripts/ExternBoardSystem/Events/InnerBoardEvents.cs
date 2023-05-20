using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine;

namespace ExternBoardSystem.Events {
    internal static class InnerBoardEvents {
        public const string OnElementAdded = "InnerBoardOnElementAdded";
        public const string OnElementRemoved = "InnerBoardOnElementRemoved";
        public const string OnElementAddFailed = "InnerBoardOnElementAddFailed";

        public const string OnCreateBoard = "InnerBoardOnCreateBoard";
    }

    internal class OnElementEventData<TElement> : EventParams {
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

        public OnBoardEventData(IBoard<TElement> board) {
            Board = board;
        }
    }
}