using System;
using System.Collections.Generic;
using Main.Managers;
using Main.Traits;
using OldGridSystem.Tiles;
using UnityEngine;

namespace OldGridSystem.Grid {
    public abstract class OldGrid : MonoBehaviour, IGrid, IGameStateResponder {
    
        [SerializeField] protected int width;
        [SerializeField] protected int height;
        [SerializeField] protected Transform origin;
        [SerializeField] protected Tile tilePrefab;

        private Vector3 _mouseOffsetForDrag;

        public static event Action GridInitDone;
        public static event Action GridDisabled;
        public static event Action GridEnabled;
        
        public static OldGrid Instance { get; private set; }
    
        public GridType Type { get; protected set; }
        // public int Count { get; protected set; }

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
            // GameManager.OnAfterGameStateChanged += HandleAfterGameStateChanged;
        }
        
        protected virtual void OnDestroy() {
            Tile.OnTileDragEvent -= DragGrid;
            Tile.OnTileMouseDownEvent -= SetMouseOffsetForDrag;
            // GameManager.OnAfterGameStateChanged -= HandleAfterGameStateChanged;
        }

        public void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null) {
            switch (state) {
                case GameState.InitGrid:
                    CreateGrid();
                    break;
                case GameState.PlayerTurn:
                    EnableGridInteractions();
                    break;
                case GameState.StoryTurn:
                case GameState.Win:
                case GameState.Lose:
                case GameState.BoardEffectTurn:
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

        public abstract int CountNeurons(ETrait trait);

        public abstract float CountNeuronsNormalized(ETrait trait);

        public abstract int MaxNeuronsPerTrait();

        public abstract int CountNeurons();

        public abstract IEnumerable<Tile> GetNeighbors(Tile tile);

        protected abstract void DisableGridInteractions();
        protected abstract void EnableGridInteractions();
    }
}