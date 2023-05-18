using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    public abstract class MUIElementPlacer<T> : MonoBehaviour {
        // subscribe to events
        [SerializeField] private MBoardController controller;
        [SerializeField] private MBoardElementsController elementsController;

        protected Tilemap TileMap { get; private set; }
        
        protected virtual void Awake() {
            TileMap = GetComponentInChildren<Tilemap>();
            controller.OnCreateBoard += OnCreateBoard;
            elementsController.OnAddElement += OnAddElement;
            elementsController.OnRemoveElement += OnRemoveElement;
        }

        protected abstract void OnCreateBoard(IBoard board);
        protected abstract void OnRemoveElement(BoardElement element, Vector3Int cell);
        protected abstract void OnAddElement(BoardElement element, Vector3Int cell);
    }
}