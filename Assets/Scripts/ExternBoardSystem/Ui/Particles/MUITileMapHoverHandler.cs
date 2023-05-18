using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Particles {
    [RequireComponent(typeof(Tilemap))]
    public class MUITileMapHoverHandler : MonoBehaviour {
        private Camera Camera { get; set; }
        private Tilemap TileMap { get; set; }
        private IHoverEffect Hover { get; set; }

        private void Awake() {
            Camera = Camera.main;
            TileMap = GetComponent<Tilemap>();
            Hover = GetComponentInChildren<IHoverEffect>();
        }

        private void HideHover() {
            Hover.Hide();
        }

        private void ShowHover(Vector3 position) {
            Hover.Show(position);
        }

        private void Update() {
            CalculateHoverPosition();
        }

        private void CalculateHoverPosition() {
            var mousePosition = Input.mousePosition;
            var worldHoverPosition = Camera.ScreenToWorldPoint(mousePosition);
            var cellPosition = TileMap.WorldToCell(worldHoverPosition);
            var hasTile = TileMap.HasTile(cellPosition);
            if (!hasTile) {
                HideHover();
                return;
            }

            var worldCellPosition = TileMap.CellToWorld(cellPosition);
            ShowHover(worldCellPosition);
        }
    }
}