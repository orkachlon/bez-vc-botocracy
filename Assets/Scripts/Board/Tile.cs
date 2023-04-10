using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Board {
    public class Tile : MonoBehaviour {

        public static Action<Tile> TileDraggedEvent;
        public static Action<Vector3> OnTileMouseDownEvent;

        public int ID {
            get;
        }

        private Neuron _occupant;
        private SpriteRenderer _spriteRenderer;
        public Color TileColor { get; set; }

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            TileColor = Color.white;
        }

        private void OnMouseEnter() {
            _spriteRenderer.color = Color.blue;
        }

        private void OnMouseExit() {
            _spriteRenderer.color = TileColor;
        }

        private void OnMouseDrag() {
            TileDraggedEvent?.Invoke(this);
        }

        private void OnMouseDown() {
            OnTileMouseDownEvent?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        public void Init() {
            _spriteRenderer.color = TileColor;
        }

        public bool IsEmpty() {
            return _occupant != null;
        }
    }
}