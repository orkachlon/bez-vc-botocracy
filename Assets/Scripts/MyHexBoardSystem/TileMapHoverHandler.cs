using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem {
    [RequireComponent(typeof(Tilemap))]
    public class TileMapHoverHandler : MonoBehaviour {
        private Camera Camera { get; set; }
        private Tilemap TileMap { get; set; }
        private IHoverEffect HoverEffect { get; set; }

        public static event Action<Vector3> OnMouseHoverTile; 

        private void Awake() {
            Camera = Camera.main;
            TileMap = GetComponent<Tilemap>();
            HoverEffect = GetComponent<IHoverEffect>();
        }

        private void HideHover() {
            HoverEffect.Hide();
        }

        private void ShowHover(Vector3 position) {
            HoverEffect.Show(position);
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
            OnMouseHoverTile?.Invoke(worldCellPosition);
        }
    }
}