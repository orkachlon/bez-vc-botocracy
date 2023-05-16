using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using Traits;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Utils;
using Tile = Tiles.Tile;

namespace Grids {
    public class HexGrid : Grid {

        [SerializeField] private List<Color> tileColors;
        [SerializeField] private int gridRadius = 4;
        [Header("Labels")]
        [SerializeField] private float labelOffsetFromGrid;

        [SerializeField] private TextMeshPro labelPrefab;
        
        private Tile _startingTile;
        private Dictionary<Vector2, Tile> _tiles;
        private Dictionary<int, List<Tile>> _tilesByRadius;
        private Dictionary<ETraitType, List<Tile>> _tilesByTrait;
        private Dictionary<Tuple<int, int>, Tile> _hexTiles;
        
        protected override void Awake() {
            _hexTiles = new Dictionary<Tuple<int, int>, Tile>();
            // CreateHexagonalGridAlt();
        }

        protected void Start() {
            Type = GridType.Hex;
        }

        public override IEnumerable<Tile> GetNeighbors(Tile tile) {
            throw new NotImplementedException();
        }

        #region GridInteraction

        protected override void DisableGridInteractions() {
            InteractionDisabled = true;
            foreach (var tile in _tiles.Values) {
                tile.Disable();
            }
            _startingTile.Disable();
        }

        protected override void EnableGridInteractions() {
            InteractionDisabled = false;
            foreach (var tile in _tiles.Values) {
                tile.Enable();
            }
            _startingTile.Enable();
        }

        #endregion

        #region NeuronCounting

        public override int CountNeurons(ETraitType trait) {
            return _tilesByTrait[trait].Count(t => !t.IsEmpty());
        }

        public override int MaxNeuronsPerTrait() {
            return _tilesByTrait[0].Count;
        }

        public override float CountNeuronsNormalized(ETraitType trait) {
            return (float) _tilesByTrait[trait].Count(t => !t.IsEmpty()) / _tilesByTrait[trait].Count;
        }

        public override int CountNeurons() {
            // first neuron is placed automatically
            return 1 + _tiles.Values.Count(t => !t.IsEmpty());
        }

        #endregion

        #region GridCreation

        public override void CreateGrid() {
            _tiles = new Dictionary<Vector2, Tile>();
            _tilesByRadius = new Dictionary<int, List<Tile>>();
            _tilesByTrait = new Dictionary<ETraitType, List<Tile>>();
            // CreateRectangularGrid();
            CreateHexagonalGrid();
            PlaceLabels();
            NeuronManager.Instance.PlaceFirstNeuron(_startingTile);
            OnGridCreated();
        }

        private void PlaceLabels() {
            var angle = 30;
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                var direction = Quaternion.AngleAxis(angle, Vector3.back) * Vector3.up;
                var rotation = Quaternion.LookRotation(Vector3.forward, Mathf.Abs(angle) > 90 ? -direction.normalized : direction.normalized);
                var labelPos = origin.position + direction *
                    (gridRadius * _tilesByTrait[trait][0].Width * 0.75f + labelOffsetFromGrid);
                var label = Instantiate(labelPrefab, labelPos, rotation, transform);
                label.text = trait.ToString();
                angle -= 60;
            }
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

        private void CreateHexagonalGridAlt() {
            float tileOuterR = 2;
            float tileInnerR = Mathf.Sqrt(tileOuterR);
            var rowStart = Vector3.zero;
            for (var q = -2; q <= 2; q++) {
                for (var r = Mathf.Max(-2, -q -2); r <= Mathf.Min(2, 2 - q); r++) {
                    var s = -q - r;
                    var tilePos = new Vector3(q * 1.5f * tileOuterR, rowStart.y + r * tileInnerR);
                    // _hexTiles[new Tuple<int, int>(q, r)] = InstantiateTile(tilePos, new Tuple<int, int>(q, r));

                }

                rowStart += new Vector3(tileOuterR * 1.5f, tileInnerR);
            }
        }

        // private Tile InstantiateTile(Vector3 position, Tuple<int, int> qr) {
        //     var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        //     _hexTiles[qr] = tile;
        // }

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
            _startingTile.TileBaseColor = tileColors[2];
            return _startingTile;
        }

        private Tile InstantiateTile(Vector3 position, int tileIndex, int radius) {
#if UNITY_EDITOR
            Assert.IsFalse(radius == 0);
#endif
            var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            var edge = Mathf.CeilToInt((float) tileIndex / radius);
            tile.name = $"{tileIndex}, {radius}";
            tile.TileBaseColor = tileColors[edge % 2];
            
            // add to elementData structures
            _tiles[new Vector2(tileIndex, radius)] = tile;
            if (tileIndex == 0) {
                _tilesByRadius[radius] = new List<Tile>() {tile};
            }
            else {
                _tilesByRadius[radius].Add(tile);
            }

            var trait = EnumUtil.GetValues<ETraitType>().ToList()[edge % 6];
            if (_tilesByTrait.ContainsKey(trait)) {
                _tilesByTrait[trait].Add(tile);
            }
            else {
                _tilesByTrait[trait] = new List<Tile>() {tile};
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
            tile.TileBaseColor = tileColors[0];
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

                    tile.TileBaseColor = tileColors[(i + j) % tileColors.Count];

                    // manage internals
                    _tiles[new Vector2(i, j)] = tile;
                    prevTile = j < width - 1 ? tile : _tiles[new Vector2(i, 0)];
                }
            }
        }

        #endregion
    }
}