using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    public abstract class MUIElementPlacer<T, TUI> : MonoBehaviour where T : BoardElement{
        // subscribe to events
        [SerializeField] private MBoardController<T> controller;
        [SerializeField] private MBoardElementsController<T> elementsController;

        protected Tilemap TileMap { get; private set; }
        
        protected virtual void Awake() {
            TileMap = GetComponentInChildren<Tilemap>();
            controller.OnCreateBoard += OnCreateBoard;
            elementsController.OnAddElement += OnAddElement;
            elementsController.OnRemoveElement += OnRemoveElement;
        }

        protected abstract void OnCreateBoard(IBoard<T> board);
        protected abstract void OnRemoveElement(BoardElement element, Vector3Int cell);
        protected abstract void OnAddElement(BoardElement element, Vector3Int cell);
    }
}