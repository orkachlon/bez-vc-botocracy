using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Tile = Tiles.Tile;

namespace Grids {
    public class HexGrid : Grid {

        [SerializeField] private List<Color> tileColors;
        [SerializeField] private int gridRadius = 4;

        private Tile _startingTile;
        private Dictionary<Vector2, Tile> _tiles;
        private Dictionary<int, List<Tile>> _tilesByRadius;
        private Dictionary<int, List<Tile>> _tilesByEdge;

        protected override void Start() {
            base.Start();
            Type = GridType.Hex;
            // CreateGrid();
        }
    
        public override void CreateGrid() {
            _tiles = new Dictionary<Vector2, Tile>();
            _tilesByRadius = new Dictionary<int, List<Tile>>();
            _tilesByEdge = new Dictionary<int, List<Tile>>();
            // CreateRectangularGrid();
            CreateHexagonalGrid();
        }
        
        private void CreateHexagonalGrid() {
            var tile = InstantiateStartingTile();
            var prevTile = tile;
            for (var currRadius = 1; currRadius < gridRadius; currRadius++) {
                var tilePos = prevTile.transform.position + Vector3.up * 2 * prevTile.Radius;
                tile = InstantiateTile(tilePos, 0, currRadius);
                prevTile = tile;
                for (var tileIndex = 1; tileIndex < currRadius * 6; tileIndex++) {
                    tilePos = GetTilePosFromPrevTile(tileIndex, currRadius, prevTile);
                    tile = InstantiateTile(tilePos, tileIndex, currRadius);
                    prevTile = tile;
                }

                prevTile = _tilesByRadius[currRadius][0];
            }
        }

        private static Vector3 GetTilePosFromPrevTile(int tileIndex, int currRadius, Tile prevTile) {
            var direction = Mathf.CeilToInt((float) tileIndex / currRadius);
            var tilePos = prevTile.transform.position;
            switch (direction) {
                case 1:
                    tilePos += Quaternion.AngleAxis(-120, Vector3.back) * Vector3.up * 2 * prevTile.Radius;
                    break;
                case 2:
                    tilePos += Vector3.down * 2 * prevTile.Radius;
                    break;
                case 3:
                    tilePos += Quaternion.AngleAxis(120, Vector3.back) * Vector3.up * 2 * prevTile.Radius;
                    break;
                case 4:
                    tilePos += Quaternion.AngleAxis(60, Vector3.back) * Vector3.up * 2 * prevTile.Radius;
                    break;
                case 5:
                    tilePos += Vector3.up * 2 * prevTile.Radius;
                    break;
                case 6:
                    tilePos += Quaternion.AngleAxis(-60, Vector3.back) * Vector3.up * 2 * prevTile.Radius;
                    break;
            }

            return tilePos;
        }

        private Tile InstantiateStartingTile() {
            _startingTile = Instantiate(tilePrefab, origin.position, Quaternion.identity, transform);
            _startingTile.name = "0, 0";
            _startingTile.TileColor = tileColors[2];
            return _startingTile;
        }

        private Tile InstantiateTile(Vector3 position, int tileIndex, int radius) {
#if UNITY_EDITOR
            Assert.IsFalse(radius == 0);
#endif
            var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            var edge = Mathf.CeilToInt((float) tileIndex / radius);
            tile.name = $"{tileIndex}, {radius}";
            tile.TileColor = tileColors[edge % 2];
            
            // add to data structures
            _tiles[new Vector2(tileIndex, radius)] = tile;
            if (tileIndex == 0) {
                _tilesByRadius[radius] = new List<Tile>() {tile};
            }
            else {
                _tilesByRadius[radius].Add(tile);
            }

            if (_tilesByEdge.ContainsKey(edge)) {
                _tilesByEdge[edge].Add(tile);
            }
            else {
                _tilesByEdge[edge] = new List<Tile>() {tile};
            }
            
            return tile;
        }

        private void CreateRectangularGrid() {
            var tile = Instantiate(tilePrefab, origin.position, Quaternion.identity, transform);
            // even: half the width * number of pairs * 1.5 of tile width
            // odd: ceil of half the width * number of pairs * 1.5 of tile width - 0.25 tile width
            var halfWidth = width % 2 == 0
                ? width * 0.375f * tile.Width
                : Mathf.CeilToInt(width * 0.5f) * 0.5f * 1.5f * tile.Width - 0.25f * tile.Width;
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