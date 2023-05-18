using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Ui.Board;
using MyHexBoardSystem.BoardElements.Neuron;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.UI {
    public class MUINeuronPlacer : MUIElementPlacer<MUIBoardNeuron> {
        private readonly Dictionary<BoardElement, MUIBoardNeuron> _registerUiElements = new();
        
        protected override void OnCreateBoard(IBoard board) {
            CreateBoardUi();
        }

        protected override void OnRemoveElement(BoardElement element, Vector3Int cell) {
            var uiElement = _registerUiElements[element];
            MObjectPooler.Instance.Release(uiElement.gameObject);
        }

        protected override void OnAddElement(BoardElement element, Vector3Int cell) {
            // think of a better way to do this
            if (typeof(BoardNeuron) != element.GetType()) {
                return;
            }
            var data = element.DataProvider;
            var model = data.GetModel();
            var uiBoardElement = MObjectPooler.Instance.Get<MUIBoardNeuron>(model);
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