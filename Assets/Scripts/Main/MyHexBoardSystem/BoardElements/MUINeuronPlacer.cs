using System.Collections.Generic;
using System.Threading.Tasks;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Ui.Board;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements {
    public class MUINeuronPlacer : MUIElementPlacer<BoardNeuron, MUIBoardNeuron> {
        private readonly Dictionary<BoardElement, MUIBoardNeuron> _registerUiElements = new();
        private Task _currentUITask;
        
        protected override void OnCreateBoard(IBoard<BoardNeuron> board) {
            CreateBoardUi();
        }

        protected override void OnRemoveElement(BoardNeuron element, Vector3Int cell) {
            _currentUITask = RemoveElementAsync(element);
        }

        protected override void OnAddElement(BoardNeuron element, Vector3Int cell) {
            _currentUITask = AddElementAsync(element, cell);
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
        }

        private async Task AddElementAsync(BoardNeuron element, Vector3Int cell) {
            await AwaitCurrentUITask();
            await Task.Delay(100);
            var data = element.DataProvider;
            var model = data.GetModel();
            var uiBoardElement = MObjectPooler.Instance.Get<MUIBoardNeuron>(model.gameObject);
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
        }

        private async Task RemoveElementAsync(BoardNeuron element) {
            await AwaitCurrentUITask();
            await Task.Delay(100);
            var uiElement = _registerUiElements[element];
            MObjectPooler.Instance.Release(uiElement.gameObject);
            _registerUiElements.Remove(element);
        }

        private async Task AwaitCurrentUITask() {
            if (_currentUITask is {IsCompleted: false}) {
                await _currentUITask;
            }
        }
    }
}