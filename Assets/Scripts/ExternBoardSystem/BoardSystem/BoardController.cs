using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private readonly HashSet<Hex> _tiles = new();
        
        public IBoard Board { get; private set; }
        public IBoardManipulation BoardManipulation { get; private set; }

        public event Action<IBoard> OnCreateBoard;
        
        private void Awake() {
            CollectExistingTiles();
        }

        private void CollectExistingTiles() {
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

        private void CreateBoard() {
            Board = new Board.Board(this,
                tilemap.orientation == Tilemap.Orientation.XY ? Orientation.PointyTop : Orientation.FlatTop);
            BoardManipulation = new BoardManipulationOddR(Board);
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
        
        public void DispatchCreateBoard(IBoard board)
        {
            OnCreateBoard?.Invoke(board);
        }

        public Hex[] GetHexPoints() {
            return _tiles.ToArray();
        }
    }
}