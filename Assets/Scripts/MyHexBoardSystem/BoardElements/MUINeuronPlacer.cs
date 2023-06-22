using System.Collections.Generic;
using System.Threading.Tasks;
using Animation;
using Core.Tools.Pooling;
using ExternBoardSystem.Ui.Board;
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
            // AnimationManager.Register(element, RemoveElementAsync(element));
        }

        protected override void OnAddElement(IBoardNeuron element, Vector3Int cell) {
            // AnimationManager.Register(element, AddElementAsync(element, cell));
        }

        protected override void OnMoveElement(IBoardNeuron element, Vector3Int fromCell, Vector3Int toCell) {
            // AnimationManager.Register(element, MoveElementAsync(element, fromCell, toCell));
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                MObjectPooler.Instance.Release(element.GO);

            _registerUiElements.Clear();
        }

        public async Task AddElementAsync(IBoardNeuron element, Vector3Int cell) {
            var uiBoardElement = element.Pool();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
            await AnimationManager.Register(element, element.AwaitAddition());
        }

        public async Task RemoveElementAsync(IBoardNeuron element) {
            await AnimationManager.Register(element, element.AwaitRemoval());
            element.Release();
            _registerUiElements.Remove(element);
        }

        public async Task MoveElementAsync(IBoardNeuron element, Vector3Int fromCell, Vector3Int toCell) {
            await AnimationManager.Register(element, element.AwaitMove(TileMap.CellToWorld(fromCell), TileMap.CellToWorld(toCell)));
        }
    }
}