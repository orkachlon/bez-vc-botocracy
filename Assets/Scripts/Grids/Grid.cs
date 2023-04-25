using System;
using Managers;
using Tiles;
using Traits;
using UnityEngine;

namespace Grids {
    public abstract class Grid : MonoBehaviour, IGameStateResponder {
    
        [SerializeField] protected int width;
        [SerializeField] protected int height;
        [SerializeField] protected Transform origin;
        [SerializeField] protected Tile tilePrefab;

        private Vector3 _mouseOffsetForDrag;

        public static event Action GridInitDone;
        public static event Action GridDisabled;
        public static event Action GridEnabled;
        
        public static Grid Instance { get; private set; }
    
        public GridType Type { get; protected set; }

        private bool _interactionDisabled;
        public bool InteractionDisabled {
            get => _interactionDisabled;
            protected set {
                _interactionDisabled = value;
                if (_interactionDisabled) {
                    GridDisabled?.Invoke();
                }
                else {
                    GridEnabled?.Invoke();
                }
            }
        }

        public enum GridType {
            Square,
            Hex
        }

        protected virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            Tile.OnTileDragEvent += DragGrid;
            Tile.OnTileMouseDownEvent += SetMouseOffsetForDrag;
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }
        
        protected virtual void OnDestroy() {
            Tile.OnTileDragEvent -= DragGrid;
            Tile.OnTileMouseDownEvent -= SetMouseOffsetForDrag;
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        public void HandleGameStateChanged(GameManager.GameState state) {
            switch (state) {
                case GameManager.GameState.InitGrid:
                    CreateGrid();
                    break;
                case GameManager.GameState.PlayerTurn:
                    EnableGridInteractions();
                    break;
                case GameManager.GameState.EventEvaluation:
                case GameManager.GameState.EventTurn:
                case GameManager.GameState.Win:
                case GameManager.GameState.Lose:
                case GameManager.GameState.StatEvaluation:
                    DisableGridInteractions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void SetMouseOffsetForDrag(Vector3 offset) {
            var currPosition = transform.position;
            _mouseOffsetForDrag = currPosition - new Vector3(offset.x, offset.y, currPosition.z);
        }

        private void DragGrid(Tile tile) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            transform.position = mousePos + _mouseOffsetForDrag;
        }

        public abstract void CreateGrid();

        protected void OnGridCreated() {
            GridInitDone?.Invoke();
        }

        public virtual int CountNeurons(ETraitType trait) {
            return -1;
        }
        
        public virtual float CountNeuronsNormalized(ETraitType trait) {
            return -1;
        }

        public abstract void DisableGridInteractions();
        public abstract void EnableGridInteractions();
    }
}