using System;
using ExternBoardSystem.Tools.Input.Mouse;
using ExternBoardSystem.Ui.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Util {
    
    /// <summary>
    ///     Receives input events on the board, and dispatches them onward with relevant data.
    /// </summary>
    [RequireComponent(typeof(IMouseInput)), RequireComponent(typeof(Tilemap)),
     RequireComponent(typeof(TilemapCollider2D))]
    public class MUITileMapInputHandler : MonoBehaviour {
        private Camera Camera { get; set; }
        private Tilemap TileMap { get; set; }
        private IMouseInput MouseInput { get; set; }

        #region Events
        public event Action<Vector3Int> OnClickTile;
        public event Action<Vector3Int, Vector2> OnRightClickTile;
        #endregion

        private void Awake() {
            Camera = Camera.main;
            TileMap = GetComponentInChildren<Tilemap>();
            MouseInput = GetComponent<IMouseInput>();
            MouseInput.OnPointerClick += OnPointerClick;
        }

        private void OnPointerClick(PointerEventData eventData) {
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

        private Vector3Int ConvertPixelToCell(Vector2 screenPoint) {
            var worldPosition = Camera.ScreenToWorldPoint(screenPoint);
            var cell = TileMap.WorldToCell(worldPosition);
            return cell;
        }
    }
}