using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utils;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Ui.Board;
using Main.Animation;
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
            AnimationManager.Register(RemoveElementAsync(element), EAnimationQueue.Neurons);
            // _currentUITask = RemoveElementAsync(element);
        }

        protected override void OnAddElement(BoardNeuron element, Vector3Int cell) {
            AnimationManager.Register(AddElementAsync(element, cell), EAnimationQueue.Neurons);
            // _currentUITask = AddElementAsync(element, cell);
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
        }

        private async Task AddElementAsync(BoardNeuron element, Vector3Int cell) {
            // await AwaitCurrentUITask();
            await Task.Delay(1000);
            var data = element.DataProvider;
            var model = data.GetModel();
            var uiBoardElement = MObjectPooler.Instance.Get<MUIBoardNeuron>(model.gameObject);
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
            MLogger.LogEditor("Added neuron!");
        }

        private async Task RemoveElementAsync(BoardNeuron element) {
            // await AwaitCurrentUITask();
            // await Task.Delay(1000);
            await Task.Yield();
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