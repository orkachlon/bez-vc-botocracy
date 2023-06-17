using System.Collections.Generic;
using Core.Tools.Pooling;
using ExternBoardSystem.BoardElements;
using Types.Board;
using UnityEngine;

namespace ExternBoardSystem.Ui.Board {
    
    /// <summary>
    ///     In charge of actually showing the stuff that's on the board using pooler.
    ///     todo: consider renaming this class
    /// </summary>
    public class MUIGeneralPlacer : MUIElementPlacer<BoardElement, MUIBoardElement> {
        private readonly Dictionary<BoardElement, MUIBoardElement> _registerUiElements = new();

        protected override void OnCreateBoard(IBoard<BoardElement> board) {
            CreateBoardUi();
        }

        protected override void OnRemoveElement(BoardElement element, Vector3Int cell) {
            var uiElement = _registerUiElements[element];
            MObjectPooler.Instance.Release(uiElement.gameObject);
        }

        protected override void OnAddElement(BoardElement element, Vector3Int cell) {
            var data = element.DataProvider;
            var model = data.GetModel();
            var obj = MObjectPooler.Instance.Get(model.GO);
            var uiBoardElement = obj.GetComponent<MUIBoardElement>();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
        }

        protected override void OnMoveElement(BoardElement element, Vector3Int fromCell, Vector3Int toCell) {
            OnRemoveElement(element, fromCell);
            OnAddElement(element, toCell);
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
        }
    }
}