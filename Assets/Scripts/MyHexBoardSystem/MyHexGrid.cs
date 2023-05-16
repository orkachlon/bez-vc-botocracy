using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem {
    public class MyHexGrid : MonoBehaviour {

        [SerializeField] private Tilemap tilemap;

        private readonly HashSet<Hex> _tiles = new();

        private void Awake() {
            tilemap.CompressBounds();

            var area = tilemap.cellBounds;
            foreach (var pos in area.allPositionsWithin) {
                if (tilemap.HasTile(pos)) {
                    // convert to Hex and save Map[Hex] = tile
                    _tiles.Add(OffsetCoordHelper.RoffsetToCube(OffsetCoord.Parity.Odd, new OffsetCoord(pos.x, pos.y)));
                }
            }
            print(_tiles.Count);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            foreach (var cell in _tiles
                .Select(tile => OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, tile))
                .Select(offset => tilemap.CellToWorld(offset.ToVector3Int()))) {
                Gizmos.DrawWireSphere(cell, 0.5f);
            }
        }
#endif
    }
}