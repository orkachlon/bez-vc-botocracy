using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    public abstract class MUIElementPlacer<T, TUI> : MonoBehaviour 
        where T : BoardElement
        where TUI : MUIBoardElement {
        
        // subscribe to events
        [SerializeField] private MBoardController<T> controller;
        [SerializeField] private MBoardElementsController<T, TUI> elementsController;

        [Header("Events"), SerializeField] private SEventManager innerBoardEventManager;

        protected Tilemap TileMap { get; private set; }
        
        protected virtual void Awake() {
            TileMap = GetComponentInChildren<Tilemap>();
            innerBoardEventManager.Register(InnerBoardEvents.OnElementAdded, OnAddElement);
            innerBoardEventManager.Register(InnerBoardEvents.OnElementRemoved, OnRemoveElement);
            innerBoardEventManager.Register(InnerBoardEvents.OnCreateBoard, OnCreateBoard);
            // controller.OnCreateBoard += OnCreateBoard;
            // elementsController.OnAddElement += OnAddElement;
            // elementsController.OnRemoveElement += OnRemoveElement;
        }

        #region EventHandlers

        protected virtual void OnAddElement(EventParams eventData) {
            if (eventData is OnElementEventData<T> elementEventData) {
                OnAddElement(elementEventData.Element, elementEventData.Cell);
            }
        }
        
        protected virtual void OnCreateBoard(EventParams eventData) {
            if (eventData is OnBoardEventData<T> boardEventData) {
                OnCreateBoard(boardEventData.Board);
            }
        }

        protected virtual void OnRemoveElement(EventParams eventData) {
            if (eventData is OnElementEventData<T> elementEventData) {
                OnRemoveElement(elementEventData.Element, elementEventData.Cell);
            }
        }

        #endregion

        protected abstract void OnCreateBoard(IBoard<T> board);
        protected abstract void OnRemoveElement(T element, Vector3Int cell);
        protected abstract void OnAddElement(T element, Vector3Int cell);
    }
}