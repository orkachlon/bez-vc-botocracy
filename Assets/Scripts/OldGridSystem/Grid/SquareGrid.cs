using System;
using System.Collections.Generic;
using Main.Traits;
using OldGridSystem.Tiles;
using UnityEngine;

namespace OldGridSystem.Grid {
    public class SquareGrid : OldGrid {

        private Dictionary<Tuple<int, int>, Tile> _tiles;

        protected void Start() {
            Type = GridType.Square;
            _tiles = new Dictionary<Tuple<int, int>, Tile>();
        }

        public override void CreateGrid() {
            for (var i = 0; i < height; i++) {
                for (var j = 0; j < width; j++) {
                    // offset the grid from the origin to center it
                    var gridOffset = origin.position - new Vector3((float)width / 2, (float)height / 2);
                    var tile = Instantiate(tilePrefab, gridOffset, Quaternion.identity, transform);
                    // place tile in grid position
                    tile.transform.position += new Vector3(
                        2 * tile.Radius * j + tile.Radius,
                        2 * tile.Radius * i + tile.Radius
                    );
                    tile.name = $"{i}, {j}";

                    if ((i + j) % 2 == 1) {
                        tile.TileBaseColor = Color.black;
                    }

                    tile.Init();

                    _tiles[new Tuple<int, int>(i, j)] = tile;
                }
            }
            OnGridCreated();
        }

        #region NotImplemeted

        public override int CountNeurons(ETrait trait) {
            throw new NotImplementedException();
        }

        public override float CountNeuronsNormalized(ETrait trait) {
            throw new NotImplementedException();
        }

        public override int MaxNeuronsPerTrait() {
            throw new NotImplementedException();
        }

        public override int CountNeurons() {
            throw new NotImplementedException();
        }

        public override IEnumerable<Tile> GetNeighbors(Tile tile) {
            throw new NotImplementedException();
        }

        protected override void DisableGridInteractions() {
            throw new NotImplementedException();
        }

        protected override void EnableGridInteractions() {
            throw new NotImplementedException();
        }

        #endregion
    }
}