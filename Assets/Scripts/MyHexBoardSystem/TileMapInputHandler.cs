using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem {
    [RequireComponent(typeof(Tilemap)), RequireComponent(typeof(TilemapCollider2D))]
    public class TileMapInputHandler : MonoBehaviour, IPointerClickHandler {
        private Camera Camera { get; set; }
        private Tilemap TileMap { get; set; }
        
        public event Action<Vector3Int> OnClickTile;
        public event Action<Vector3Int, Vector2> OnRightClickTile;

        public void OnPointerClick(PointerEventData eventData) {
            var screenPosition = eventData.position;
            var cell = ConvertPixelToCell(screenPosition);
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    OnClickTile?.Invoke(cell);
                    break;
                case PointerEventData.InputButton.Right:
                    OnRightClickTile?.Invoke(cell, screenPosition);
                    break;
            }
        }

        private void Awake() {
            Camera = Camera.main;
            TileMap = GetComponent<Tilemap>();
        }

        private Vector3Int ConvertPixelToCell(Vector2 screenPoint) {
            var worldPosition = Camera.ScreenToWorldPoint(screenPoint);
            var cell = TileMap.WorldToCell(worldPosition);
            return cell;
        }
    }
}