using System.Collections.Generic;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Position;
using ExternBoardSystem.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    
    /// <summary>
    ///     In charge of actually showing the stuff that's on the board using pooler.
    ///     todo: consider renaming this class
    /// </summary>
    public class UiBoard : MonoBehaviour {
        private readonly Dictionary<BoardElement, UiBoardElement> _registerUiElements = new();

        [SerializeField] private BoardController controller;
        [SerializeField] private BoardElementsController elementsController;
        
        private IBoard CurrentBoard { get; set; }
        private Tilemap TileMap { get; set; }

        private void Awake() {
            TileMap = GetComponentInChildren<Tilemap>();
            controller.OnCreateBoard += OnCreateBoard;
            elementsController.OnAddElement += OnAddElement;
            elementsController.OnRemoveElement += OnRemoveElement;
        }

        private void OnCreateBoard(IBoard board) {
            CurrentBoard = board;
            CreateBoardUi();
        }

        private void OnRemoveElement(BoardElement element, Vector3Int cell) {
            var uiElement = _registerUiElements[element];
            ObjectPooler.Instance.Release(uiElement.gameObject);
        }

        private void OnAddElement(BoardElement element, Vector3Int cell) {
            var data = element.DataProvider;
            var model = data.GetModel();
            var obj = ObjectPooler.Instance.Get(model);
            var uiBoardElement = obj.GetComponent<UiBoardElement>();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
        }

        private void CreateBoardUi() {
            foreach (var element in _registerUiElements.Values)
                ObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
        }
    }
}