using System.Collections.Generic;
using System.Threading.Tasks;
using Animation;
using Core.Tools.Pooling;
using DG.Tweening;
using ExternBoardSystem.Ui.Board;
using Main.Animation;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Types.Board;
using Types.Board.UI;
using Types.Neuron.Runtime;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements {
    public class MUINeuronPlacer : MUIElementPlacer<IBoardNeuron, IUIBoardNeuron> {
        private readonly Dictionary<IBoardElement, IUIBoardNeuron> _registerUiElements = new();
        
        protected override void OnCreateBoard(IBoard<IBoardNeuron> board) {
            CreateBoardUi();
        }

        protected override void OnRemoveElement(IBoardNeuron element, Vector3Int cell) {
            AnimationManager.Register(element, RemoveElementAsync(element));
        }

        protected override void OnAddElement(IBoardNeuron element, Vector3Int cell) {
            AnimationManager.Register(element, AddElementAsync(element, cell));
        }

        protected override void OnMoveElement(IBoardNeuron element, Vector3Int fromCell, Vector3Int toCell) {
            AnimationManager.Register(element, MoveElementAsync(element, fromCell, toCell));
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.GO);

            _registerUiElements.Clear();
        }

        private async Task AddElementAsync(IBoardNeuron element, Vector3Int cell) {
            var uiBoardElement = element.Pool();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
            await uiBoardElement.PlayAddAnimation();
        }

        private async Task RemoveElementAsync(IBoardNeuron element) {
            var uiElement = _registerUiElements[element];
            await uiElement.PlayRemoveAnimation();
            MObjectPooler.Instance.Release(uiElement.GO);
            _registerUiElements.Remove(element);
        }

        private async Task MoveElementAsync(IBoardNeuron element, Vector3Int fromCell, Vector3Int toCell) {
            var uiElement = _registerUiElements[element];
            uiElement.GO.transform.DOMove(TileMap.CellToWorld(toCell), 0.5f);
            await uiElement.PlayMoveAnimation();
        }
    }
}