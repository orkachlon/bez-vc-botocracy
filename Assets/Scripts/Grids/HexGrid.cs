using System.Collections.Generic;
using System.Linq;
using Tiles;
using UnityEngine;

namespace Grids {
    public class HexGrid : Grid {

        [SerializeField] private List<Color> tileColors;

        private Dictionary<Vector2, Tile> _tiles;

        protected override void Start() {
            base.Start();
            Type = GridType.Hex;
            // CreateGrid();
        }
    
        public override void CreateGrid() {
            _tiles = new Dictionary<Vector2, Tile>();

            var tile = Instantiate(tilePrefab, origin, Quaternion.identity, transform);
            // even: half the width * number of pairs * 1.5 of tile width
            // odd: ceil of half the width * number of pairs * 1.5 of tile width - 0.25 tile width
            var halfWidth = width % 2 == 0 ? 
                width * 0.375f * tile.Width : 
                Mathf.CeilToInt(width * 0.5f) * 0.5f * 1.5f * tile.Width - 0.25f * tile.Width;
            tile.transform.position += new Vector3(
                -halfWidth, 
                (1.5f - height) * tile.Radius, 0);
            tile.name = "0, 0";
            tile.TileColor = tileColors[0];
            _tiles[new Vector2(0, 0)] = tile;
        
            var prevTile = tile;
            for (var i = 0; i < height; i++) {
                // skip the first tile of the grid
                for (var j = i > 0 ? 0 : 1; j < width; j++) {
                    var tilePos = prevTile.transform.position;
                    if (j == 0) {
                        tilePos += Vector3.up * prevTile.Radius * 2;
                    }
                    else if (j % 2 == 1) {
                        tilePos += Quaternion.AngleAxis(120, Vector3.back) * Vector3.up * tile.Radius * 2;
                    }
                    else {
                        tilePos += Quaternion.AngleAxis(60, Vector3.back) * Vector3.up * tile.Radius * 2;
                    }
                
                    // instantiate
                    tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, transform);
                    tile.name = $"{i}, {j}";

                    tile.TileColor = tileColors[(i + j) % tileColors.Count];
                    
                    // manage internals
                    _tiles[new Vector2(i, j)] = tile;
                    prevTile = j < width - 1 ? tile : _tiles[new Vector2(i, 0)];
                }
            }
        }

        public override Tile GetNearestTile(Vector3 position) {
            // todo: add nullchecks
            return _tiles.Where(t => t.Value.IsInsideTile(position)).ToList()[0].Value;
        }
    }
}