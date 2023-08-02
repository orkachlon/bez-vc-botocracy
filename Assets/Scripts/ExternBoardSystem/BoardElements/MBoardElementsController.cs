using System;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using ExternBoardSystem.Events;
using Types.Board;
using Types.Board.UI;
using Types.Hex.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardElements {
    
    /// <summary>
    ///     Places board elements on the board. A mediator between an input event and the board.
    /// </summary>
    public class MBoardElementsController<TElement, TUIElement> : 
        MonoBehaviour, IBoardElementsController<TElement> 
        where TElement : IBoardElement
        where TUIElement : IUIBoardElement {
        
        public IBoardManipulation Manipulator { get; private set; }
        public IBoard<TElement> Board { get; private set; }
        private IElementDataProvider<TElement, TUIElement> ElementProvider { get; set; }
        
        [Header("Inner Event Managers"), SerializeField] protected SEventManager externalEventManager;
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

        public virtual Task AddElement(TElement element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return Task.CompletedTask;
            if (position.HasData())
                return Task.CompletedTask;
            position.AddData(element);
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementAdded, new OnElementEventData<TElement>(element, GetCellCoordinate(hex)));
            return Task.CompletedTask;
        }
        

        public virtual Task RemoveElement(Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null || !position.HasData())
                return Task.CompletedTask;
            var data = position.Data;
            position.RemoveData();
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementRemoved, new OnElementEventData<TElement>(data, GetCellCoordinate(hex)));
            return Task.CompletedTask;
        }

        public virtual Task MoveElement(Hex from, Hex to) {
            if (!Board.HasPosition(from) || !Board.GetPosition(from).HasData() ||
                !Board.HasPosition(to) || Board.GetPosition(to).HasData()) {
                return Task.CompletedTask;
            }

            var element = Board.GetPosition(from).Data;
            Board.GetPosition(from).RemoveData();
            Board.GetPosition(to).AddData(element);
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementMoved, new OnElementMovedEventData<TElement>(element, GetCellCoordinate(from), GetCellCoordinate(to)));
            return Task.CompletedTask;
        }

        #region EventHandlers

        public void OnCreateBoard(EventArgs eventData) {
            if (eventData is OnBoardEventData<TElement> boardData) {
                OnCreateBoard(boardData.Board, boardData.Manipulator);
            }
        }

        private void OnClickTile(EventArgs eventData) {
            if (eventData is InputEventData inputEventData) {
                OnClickTile(inputEventData.Cell);
            }
        }

        #endregion

        #region EventDispatchers

        protected void DispatchOnAddElement(TElement element, Vector3Int cell) {
            innerBoardEventManager.Raise(InnerBoardEvents.OnElementAdded, new OnElementEventData<TElement>(element, cell));
        }

        protected void DispatchOnAddElementFailed(TElement element, Vector3Int cell) {
            MLogger.LogEditor($"Failed to add element! {element} in cell {cell}");
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