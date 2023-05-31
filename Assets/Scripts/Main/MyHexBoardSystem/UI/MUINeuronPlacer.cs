using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Ui.Board;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using UnityEngine;

namespace Main.MyHexBoardSystem.UI {
    public class MUINeuronPlacer : MUIElementPlacer<BoardNeuron, MUIBoardNeuron> {
        private readonly Dictionary<BoardElement, MUIBoardNeuron> _registerUiElements = new();
        
        protected override void OnCreateBoard(IBoard<BoardNeuron> board) {
            CreateBoardUi();
        }

        protected override void OnRemoveElement(BoardNeuron element, Vector3Int cell) {
            var uiElement = _registerUiElements[element];
            MObjectPooler.Instance.Release(uiElement.gameObject);
        }

        protected override void OnAddElement(BoardNeuron element, Vector3Int cell) {
            var data = element.DataProvider;
            var model = data.GetModel();
            var uiBoardElement = MObjectPooler.Instance.Get<MUIBoardNeuron>(model.gameObject);
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