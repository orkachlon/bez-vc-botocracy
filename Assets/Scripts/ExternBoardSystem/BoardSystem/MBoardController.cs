using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils.DataStructures;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Events;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.BoardSystem {
    
    /// <summary>
    ///     The board controller creates the board on startup, and through its board manipulation other
    ///     classes can ask for hex board algorithms (e.g. getNeighbors)
    /// </summary>
    public class MBoardController<T> : MonoBehaviour, IBoardController<T> where T : IBoardElement {
        [Header("Base Tilemap"), SerializeField] protected TilemapLayers tilemapLayers;

        [Header("Base Event Managers"), SerializeField]
        private SEventManager innerBoardEventManager;
        
        private readonly HashSet<Hex> _tiles = new();
        
        public const string BaseTilemapLayer = "base";
        
        public IBoard<T> Board { get; private set; }
        public IBoardManipulation Manipulator { get; private set; }

        /// <summary>
        ///     DO NOT USE
        /// </summary>
        public event Action<IBoard<T>> OnCreateBoard;
        
        protected virtual void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(tilemapLayers);
#endif
            CollectExistingTiles();
            CreateBoard();
        }

        protected virtual void Start() {
            OnCreateBoard?.Invoke(Board);
            innerBoardEventManager.Raise(InnerBoardEvents.OnCreateBoard, new OnBoardEventData<T>(Board, Manipulator));
        }

        protected virtual void CollectExistingTiles() {
            tilemapLayers[BaseTilemapLayer].CompressBounds();

            var area = tilemapLayers[BaseTilemapLayer].cellBounds;
            foreach (var pos in area.allPositionsWithin) {
                if (tilemapLayers[BaseTilemapLayer].HasTile(pos)) {
                    // convert to Hex and save Map[Hex] = tile
                    _tiles.Add(BoardManipulationOddR<T>.GetHexCoordinate(pos));
                }
            }
        }

        private void CreateBoard() {
            Board = new Board<T>(this,
                tilemapLayers[BaseTilemapLayer].orientation == Tilemap.Orientation.XY ? EOrientation.PointyTop : EOrientation.FlatTop);
            Manipulator = new BoardManipulationOddR<T>(Board);
        }

        public Hex[] GetHexPoints() {
            return _tiles.ToArray();
        }
    }

    [Serializable]
    public class TilemapLayers : UDictionary<string, Tilemap> { }
}