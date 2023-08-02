using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils.DataStructures;
using Events.Board;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<IBoardNeuron>, INeuronBoardController {


        [Header("Tilemap"), SerializeField] private TraitTiles traitTileBases;
        [SerializeField] private TraitTiles traitTileOutlines;

        [Header("Event Managers"), SerializeField]
        protected SEventManager externalBoardEventManager;

        protected ITraitAccessor _traitAccessor;

        protected readonly object BoardLock = new();


        protected override void CollectExistingTiles() {
            base.CollectExistingTiles();

            foreach (var hex in GetHexPoints()) {
                var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
                if (!trait.HasValue) {
                    continue;
                }
                tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileBases[trait.Value]);
            }
        }

        #region UnityMethods

        protected override void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
            base.Awake();
        }

        protected override void Start() {
            base.Start();
            externalBoardEventManager.Raise(ExternalBoardEvents.OnBoardSetupComplete, EventArgs.Empty);
        }

        #endregion

        #region InterfaceMethods

        public virtual int GetTraitTileCount(ETrait trait) {
            return Manipulator.GetTriangle(_traitAccessor.TraitToDirection(trait)).Count(h => Board.HasPosition(h));
        }

        public void SetColor(Hex[] hexTiles, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            foreach (var hexTile in hexTiles) {
                SetColor(hexTile, color, tilemapLayer);
            }
        }

        public void SetColor(Hex hexTile, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var tilemap = GetTilemap(tilemapLayer);
            if (tilemap == null) {
                return;
            }
            var offsetCoord = OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hexTile);
            tilemap.RemoveTileFlags(offsetCoord.ToVector3Int(), TileFlags.LockColor);
            tilemap.SetColor(offsetCoord.ToVector3Int(), color);
        }

        public TileBase GetTile(Hex hex, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var tilemap = GetTilemap(tilemapLayer);
            return tilemap == null ?
                null : 
                tilemap.GetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex));
        }

        public void SetTile(Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var tilemap = GetTilemap(tilemapLayer);
            if (tilemap == null) {
                return;
            }
            var cell = OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hexTile).ToVector3Int();
            tilemap.SetTile(cell, tile);
        }

        public void SetTiles(Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            foreach (var hex in hexTiles) {
                SetTile(hex, tile, tilemapLayer); 
            }
        }

        public Color GetColor(Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var tilemap = GetTilemap(tilemapLayer);
            return tilemap == null ? 
                Color.clear : 
                tilemap.GetColor(OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, tile).ToVector3Int());
        }

        public Hex WorldPosToHex(Vector3 position) {
            return BoardManipulationOddR<BoardNeuron>.GetHexCoordinate(tilemapLayers[BoardConstants.BaseTilemapLayer].WorldToCell(position));
        }

        public Vector3 HexToWorldPos(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            if (tilemap == null) {
                return Vector3.zero;
            }
            var cell = BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex);
            return tilemap.CellToWorld(cell);
        }

        public virtual Task RemoveTile(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            var outlineTilemap = GetTilemap(BoardConstants.OutlineTilemapLayer);
            if (!Board.HasPosition(hex) || tilemap == null || outlineTilemap == null) {
                return Task.CompletedTask;
            }
            // notify that tile is being removed before removing it
            externalBoardEventManager.Raise(ExternalBoardEvents.OnRemoveTile, new OnTileModifyEventArgs(hex));
            lock (BoardLock) {
                Board.RemovePosition(hex);
                tilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
                RecalculateTiles();
                // outlines and fills are done separately
                outlineTilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
            }
            return Task.CompletedTask;
        }

        public virtual Task AddTile(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            var outlineTilemap = GetTilemap(BoardConstants.OutlineTilemapLayer);
            var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue || tilemap == null || outlineTilemap == null) {
                return Task.CompletedTask;
            }
            lock (BoardLock) {
                Board.AddPosition(hex);
                tilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileBases[trait.Value]);
                outlineTilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileOutlines[trait.Value]);
            }
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex));
            return Task.CompletedTask;
        }

            public void RecalculateTiles() {
                var tilemap = GetTilemap(BaseTilemapLayer);
                if (tilemap == null) {
                    return;
                }
                tilemap.RefreshAllTiles();
                tilemap.CompressBounds();
        }

        public TileBase GetTraitTileBase(ETrait trait) {
            return traitTileBases[trait];
        }

        #endregion

        protected Tilemap GetTilemap(string layer) {
            if (tilemapLayers != null && tilemapLayers.ContainsKey(layer) && tilemapLayers[layer] != null) {
                return tilemapLayers[layer];
            }

            return null;
        }
    }
    
    [Serializable]
    internal class TraitTiles : UDictionary<ETrait, TileBase> { }
}