using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.BoardSystem {
    
    /// <summary>
    ///     The board controller creates the board on startup, and through its board manipulation other
    ///     classes can ask for hex board algorithms (e.g. getNeighbors)
    /// </summary>
    public class MBoardController<T> : MonoBehaviour, IBoardController<T> where T : BoardElement {
        [SerializeField] private Tilemap tilemap;

        private readonly HashSet<Hex> _tiles = new();
        
        public IBoard<T> Board { get; private set; }
        public IBoardManipulation BoardManipulation { get; private set; }

        public event Action<IBoard<T>> OnCreateBoard;
        
        protected virtual void Awake() {
            CollectExistingTiles();
        }

        protected void Start() {
            CreateBoard();
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
        }

        private void CreateBoard() {
            Board = new Board<T>(this,
                tilemap.orientation == Tilemap.Orientation.XY ? EOrientation.PointyTop : EOrientation.FlatTop);
            BoardManipulation = new BoardManipulationOddR<T>(Board);
            OnCreateBoard?.Invoke(Board);
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
        
        public void DispatchCreateBoard(IBoard<T> board) {
            // OnCreateBoard?.Invoke(board);
        }

        public Hex[] GetHexPoints() {
            return _tiles.ToArray();
        }
    }
}