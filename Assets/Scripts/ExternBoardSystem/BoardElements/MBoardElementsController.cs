using System;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Ui.Util;
using UnityEngine;

namespace ExternBoardSystem.BoardElements {
    
    /// <summary>
    ///     Places board elements on the board. A mediator between an input event and the board.
    /// </summary>
    public class MBoardElementsController : MonoBehaviour, IBoardElementsController<BoardElement> {
        [SerializeField] private MBoardController boardController;
        [SerializeField] private MUITileMapInputHandler uiTileMapInputHandler;
        public IBoardManipulation Manipulator { get; private set; }
        public IBoard Board { get; private set; }
        private IElementDataProvider<BoardElement> ElementProvider { get; set; }
        public event Action<BoardElement, Vector3Int> OnAddElement;
        public event Action<BoardElement, Vector3Int> OnRemoveElement;

        private void Awake() {
            boardController.OnCreateBoard += OnCreateBoard;
            uiTileMapInputHandler.OnClickTile += OnClickTile;
        }

        public void SetElementProvider(IElementDataProvider<BoardElement> provider) {
            ElementProvider = provider;
        }

        private void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (ElementProvider == null) {
                RemoveElement(hex);
            }
            else {
                var element = ElementProvider.GetElement();
                AddElement(element, hex);
            }
        }

        private void OnCreateBoard(IBoard board) {
            Board = board;
            Manipulator = boardController.BoardManipulation;
        }

        public void AddElement(BoardElement element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return;
            if (position.HasData())
                return;
            position.AddData(element);

            var cell = GetCellCoordinate(hex);
            OnAddElement?.Invoke(element, cell);
        }

        public void RemoveElement(Hex hex) {
            var position = Board?.GetPosition(hex);
            if (position == null)
                return;
            if (!position.HasData())
                return;
            var data = position.Data;
            position.RemoveData();
            OnRemoveElement?.Invoke(data, GetCellCoordinate(hex));
        }

        private static Hex GetHexCoordinate(Vector3Int cell) {
            return OffsetCoordHelper.RoffsetToCube(OffsetCoord.Parity.Odd, new OffsetCoord(cell.x, cell.y));
        }

        private static Vector3Int GetCellCoordinate(Hex hex) {
            return OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hex).ToVector3Int();
        }
    }
}