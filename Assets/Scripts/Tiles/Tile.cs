using System;
using Neurons;
using UnityEngine;

namespace Tiles {
    public abstract class Tile : MonoBehaviour {

        public static event Action<Tile> OnTileDragEvent;
        public static event Action<Vector3> OnTileMouseDownEvent;
        public static event Action<Tile> OnTileClickedEvent;
        public static event Action<Tile> OnTileMouseEnterEvent;
        public static event Action<Tile> OnTileMouseOverEvent;
        public static event Action<Tile> OnTileMouseExitEvent;

        [SerializeField] private float mouseClickThreshold;

        private float _mouseDownTime;
        private Collider2D _tileCollider;
        private readonly Color _tileOccupiedColor = new(.9f, .32f, .33f, 1);

        protected Neuron Occupant = null;
        protected SpriteRenderer TileRenderer;

        private Color _baseColor;
        public Color TileBaseColor {
            get => _baseColor;
            set { _baseColor = value;
                TileRenderer.color = value;
            }
        }

        public float Radius { get; private set; }
        public float Width { get; private set; }

        public bool InteractionDisabled { get; private set; }

        protected virtual void Awake() {
            TileRenderer = GetComponent<SpriteRenderer>();
            _tileCollider = GetComponent<Collider2D>();
            Radius = TileRenderer.sprite.bounds.extents.y;
            Width = TileRenderer.sprite.bounds.extents.x * 2;
            TileBaseColor = Color.white;
        }

        #region MouseInteraction

        public void Disable() {
            InteractionDisabled = true;
            Utils.Unity.SetCursorVisibility(true);
        }

        public void Enable() {
            InteractionDisabled = false;
        }
        
        private void OnMouseEnter() {
            if (InteractionDisabled) {
                return;
            }
            // if (!IsEmpty()) {
            //     TileRenderer.color = Color.red;
            // }
            // OnTileMouseEnterEvent?.Invoke(this);
        }

        private void OnMouseOver() {
            if (InteractionDisabled) {
                return;
            }
            Utils.Unity.SetCursorVisibility(false);
            if (!IsEmpty()) {
                TileRenderer.color = _tileOccupiedColor;
            }
            OnTileMouseOverEvent?.Invoke(this);

        }

        private void OnMouseExit() {
            Utils.Unity.SetCursorVisibility(true);
            if (InteractionDisabled) {
                return;
            }
            TileRenderer.color = TileBaseColor;
            OnTileMouseExitEvent?.Invoke(this);
        }

        private void OnMouseDrag() {
            if (InteractionDisabled) {
                return;
            }
            // dragging the grid is irrelevant in current version
            // OnTileDragEvent?.Invoke(this);
        }

        private void OnMouseDown() {
            if (InteractionDisabled) {
                return;
            }
            _mouseDownTime = Time.time;
            OnTileMouseDownEvent?.Invoke(Utils.Unity.GetMousePos());
        }

        private void OnMouseUpAsButton() {
            if (InteractionDisabled) {
                return;
            }
            if (Time.time - _mouseDownTime > mouseClickThreshold) {
                // not a click
                return;
            }
            OnTileClickedEvent?.Invoke(this);
        }
        #endregion

        public void Init() {
            TileRenderer.color = TileBaseColor;
        }

        public bool PlaceNeuron(Neuron neuron) {
            if (!IsEmpty()) {
                Debug.Log("Tile is occupied!");
                return false;
            }

            Occupant = neuron;
            Occupant.transform.position = transform.position;
            Occupant.transform.parent = transform;
            return true;
        }
    
        public bool IsEmpty() {
            return Occupant == null;
        }
    }
}