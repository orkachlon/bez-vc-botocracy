using System;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
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
        
        
        [SerializeField] private MBoardController<TElement> boardController;
        [SerializeField] private MUITileMapInputHandler uiTileMapInputHandler;
        public IBoardManipulation Manipulator { get; private set; }
        public IBoard<TElement> Board { get; private set; }
        private IElementDataProvider<TElement, TUIElement> ElementProvider { get; set; }
        public event Action<TElement, Vector3Int> OnAddElement;
        public event Action<TElement, Vector3Int> OnPlaceElement;
        public event Action<TElement, Vector3Int> OnAddElementFailed;
        public event Action<TElement, Vector3Int> OnRemoveElement;

        protected virtual void Awake() {
            boardController.OnCreateBoard += OnCreateBoard;
            uiTileMapInputHandler.OnClickTile += OnClickTile;
        }

        public void SetElementProvider(IElementDataProvider<TElement, TUIElement> provider) {
            ElementProvider = provider;
        }

        protected void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (ElementProvider == null) {
                return;
                // RemoveElement(hex);
            }
            var element = ElementProvider.GetElement();
            AddElement(element, hex);
            OnPlaceElement?.Invoke(element, GetCellCoordinate(hex));
        }

        private void OnCreateBoard(IBoard<TElement> board) {
            Board = board;
            Manipulator = boardController.BoardManipulation;
        }

        public void AddStartingElement(TElement element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (position.HasData()) // should never fail
                return;
            position.AddData(element);
            OnAddElement?.Invoke(element, GetCellCoordinate(hex));
        }

        public virtual void AddElement(TElement element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (position.HasData())
                return;
            position.AddData(element);
            OnAddElement?.Invoke(element, GetCellCoordinate(hex));
        }
        
        // public virtual void AddSilent

        public virtual void RemoveElement(Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (!position.HasData())
                return;
            var data = position.Data;
            position.RemoveData();
            OnRemoveElement?.Invoke(data, GetCellCoordinate(hex));
        }

        #region EventDispatchers

        protected void DispatchOnAddElement(TElement element, Vector3Int cell) {
            OnAddElement?.Invoke(element, cell);
        }

        protected void DispatchOnAddElementFailed(TElement element, Vector3Int cell) {
            OnAddElementFailed?.Invoke(element, cell);
        }

        protected void DispatchOnRemoveElement(TElement element, Vector3Int cell) {
            OnRemoveElement?.Invoke(element, cell);
        }
        #endregion

        protected static Hex GetHexCoordinate(Vector3Int cell) {
            return OffsetCoordHelper.RoffsetToCube(OffsetCoord.Parity.Odd, new OffsetCoord(cell.x, cell.y));
        }

        protected static Vector3Int GetCellCoordinate(Hex hex) {
            return OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hex).ToVector3Int();
        }
    }
}