using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Events;
using ExternBoardSystem.Ui.Board;
using ExternBoardSystem.Ui.Util;
using UnityEngine;

namespace ExternBoardSystem.BoardElements {
    
    /// <summary>
    ///     Places board elements on the board. A mediator between an input event and the board.
    /// </summary>
    public class MBoardElementsController<TElement, TUIElement> : 
        MonoBehaviour, IBoardElementsController<TElement> 
        where TElement : BoardElement
        where TUIElement : MUIBoardElement {
        
        public IBoardManipulation Manipulator { get; private set; }
        public IBoard<TElement> Board { get; private set; }
        private IElementDataProvider<TElement, TUIElement> ElementProvider { get; set; }
        
        [Header("Event Managers"), SerializeField] protected SEventManager externalEventManager;
        [SerializeField] private SEventManager innerBoardEventManager;

        protected virtual void Awake() {
            innerBoardEventManager.Register(InnerBoardEvents.OnCreateBoard, OnCreateBoard);
            innerBoardEventManager.Register(InnerBoardEvents.OnClickTile, OnClickTile);
        }

        public void SetElementProvider(IElementDataProvider<TElement, TUIElement> provider) {
            ElementProvider = provider;
        }

        protected virtual void OnClickTile(Vector3Int cell) {
            // if (ElementProvider == null) {
            //     return;
            // }
            // var element = ElementProvider.GetElement();
            // innerBoardEventManager.Raise(InnerBoardEvents.OnElementAdded, new OnElementEventData<TElement>(element, cell));
        }

        private void OnCreateBoard(IBoard<TElement> board, IBoardManipulation manipulator) {
            Board = board;
            Manipulator = manipulator;
        }

        public virtual bool AddElement(TElement element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return false;
            if (position.HasData())
                return false;
            position.AddData(element);
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementAdded, new OnElementEventData<TElement>(element, GetCellCoordinate(hex)));
            return true;
        }
        

        public virtual void RemoveElement(Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (!position.HasData())
                return;
            var data = position.Data;
            position.RemoveData();
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementRemoved, new OnElementEventData<TElement>(data, GetCellCoordinate(hex)));
        }

        #region EventHandlers

        public void OnCreateBoard(EventArgs eventData) {
            if (eventData is OnBoardEventData<TElement> boardData) {
                OnCreateBoard(boardData.Board, boardData.Manipulator);
            }
        }

        private void OnClickTile(EventArgs eventData) {
            if (eventData is OnInputEventData inputEventData) {
                OnClickTile(inputEventData.Cell);
            }
        }

        #endregion

        #region EventDispatchers

        protected void DispatchOnAddElement(TElement element, Vector3Int cell) {
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementAdded, new OnElementEventData<TElement>(element, cell));
        }

        protected void DispatchOnAddElementFailed(TElement element, Vector3Int cell) {
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementAddFailed, new OnElementEventData<TElement>(element, cell));
        }

        protected void DispatchOnRemoveElement(TElement element, Vector3Int cell) {
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementRemoved, new OnElementEventData<TElement>(element, cell));
        }
        #endregion

        #region HelperFunctions

        protected static Hex GetHexCoordinate(Vector3Int cell) {
            return OffsetCoordHelper.RoffsetToCube(OffsetCoord.Parity.Odd, new OffsetCoord(cell.x, cell.y));
        }

        protected static Vector3Int GetCellCoordinate(Hex hex) {
            return OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hex).ToVector3Int();
        }

        #endregion
    }
}