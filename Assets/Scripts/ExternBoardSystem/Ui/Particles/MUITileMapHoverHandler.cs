using System;
using ExternBoardSystem.Tools.Input.Mouse;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Particles {
    [RequireComponent(typeof(Tilemap)), RequireComponent(typeof(IMouseInput))]
    public class MUITileMapHoverHandler : MonoBehaviour {
        private Camera Camera { get; set; }
        private IMouseInput MouseInput { get; set; }
        private Tilemap TileMap { get; set; }
        private IHoverEffect Hover { get; set; }
        
        #region Events
        public event Action<Vector3Int> OnHoverTile;
        #endregion

        private void Awake() {
            Camera = Camera.main;
            TileMap = GetComponent<Tilemap>();
            Hover = GetComponentInChildren<IHoverEffect>();
            MouseInput = GetComponent<IMouseInput>();
            MouseInput.OnPointerStay += CalculateHoverPosition;
        }

        private void HideHover() {
            Hover.Hide();
        }

        private void ShowHover(Vector3 position) {
            Hover.Show(position);
        }

        private void CalculateHoverPosition(Vector2 mouseScreenPosition) {
            var worldHoverPosition = Camera.ScreenToWorldPoint(mouseScreenPosition);
            var cellPosition = TileMap.WorldToCell(worldHoverPosition);
            var hasTile = TileMap.HasTile(cellPosition);
            if (!hasTile) {
                // HideHover();
                return;
            }
            OnHoverTile?.Invoke(cellPosition);

            // var worldCellPosition = TileMap.CellToWorld(cellPosition);
            // ShowHover(worldCellPosition);
        }
    }
}