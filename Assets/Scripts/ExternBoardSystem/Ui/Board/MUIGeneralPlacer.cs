using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Tools;
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
            var obj = MObjectPooler.Instance.Get(model);
            var uiBoardElement = obj.GetComponent<MUIBoardElement>();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
        }
    }
}