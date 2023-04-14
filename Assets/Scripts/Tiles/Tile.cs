using System;
using Neurons;
using UnityEngine;

namespace Tiles {
    public abstract class Tile : MonoBehaviour {

        public static Action<Tile> OnTileDragEvent;
        public static Action<Vector3> OnTileMouseDownEvent;
        public static Action<Tile> OnTileClickedEvent;
        public static Action<Tile> OnTileMouseEnterEvent;
        public static Action<Tile> OnTileMouseOverEvent;

        [SerializeField] private float mouseClickThreshold;
    
        private float _mouseDownTime;
        private Collider2D _tileCollider;
        private readonly Color _tileOccupiedColor = new(.9f, .32f, .33f, 1);

        protected Neuron Occupant = null;
        protected SpriteRenderer SpriteRenderer;

        private Color _baseColor;
        public Color TileColor {
            get => _baseColor;
            set { _baseColor = value;
                SpriteRenderer.color = value;
            }
        }

        public float Radius { get; private set; }
        public float Width { get; private set; }

        protected virtual void Awake() {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _tileCollider = GetComponent<Collider2D>();
            Radius = SpriteRenderer.size.y / 2;
            Width = SpriteRenderer.size.x;
            TileColor = Color.white;
        }

        private void OnMouseEnter() {
            // if (!IsEmpty()) {
            //     SpriteRenderer.color = Color.red;
            // }
            // OnTileMouseEnterEvent?.Invoke(this);
        }

        private void OnMouseOver() {
            if (!IsEmpty()) {
                SpriteRenderer.color = _tileOccupiedColor;
            }
            OnTileMouseOverEvent?.Invoke(this);

        }

        private void OnMouseExit() {
            SpriteRenderer.color = TileColor;
        }

        private void OnMouseDrag() {
            OnTileDragEvent?.Invoke(this);
        }

        private void OnMouseDown() {
            _mouseDownTime = Time.time;
            OnTileMouseDownEvent?.Invoke(Utils.GetMousePos());
        }

        private void OnMouseUpAsButton() {
            if (Time.time - _mouseDownTime > mouseClickThreshold) {
                // not a click
                return;
            }
            OnTileClickedEvent?.Invoke(this);
        }

        public void Init() {
            SpriteRenderer.color = TileColor;
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

        public bool IsInsideTile(Vector3 point) {
            return _tileCollider.OverlapPoint(point);
        }
    }
}