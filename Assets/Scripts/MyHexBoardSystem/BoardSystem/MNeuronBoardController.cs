using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
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
        private SEventManager externalBoardEventManager;

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
            var offsetCoord = OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hexTile);
            tilemapLayers[tilemapLayer].RemoveTileFlags(offsetCoord.ToVector3Int(), TileFlags.LockColor);
            tilemapLayers[tilemapLayer].SetColor(offsetCoord.ToVector3Int(), color);
        }

        public TileBase GetTile(Hex hex, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            return tilemapLayers[tilemapLayer].GetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex));
        }

        public void SetTile(Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var cell = OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hexTile).ToVector3Int();
            tilemapLayers[tilemapLayer].SetTile(cell, tile);
        }

        public void SetTiles(Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            foreach (var hex in hexTiles) {
                SetTile(hex, tile, tilemapLayer); 
            }
        }

        public Color GetColor(Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            return tilemapLayers[tilemapLayer].GetColor(OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, tile).ToVector3Int());
        }

        public Hex WorldPosToHex(Vector3 position) {
            return BoardManipulationOddR<BoardNeuron>.GetHexCoordinate(tilemapLayers[BoardConstants.BaseTilemapLayer].WorldToCell(position));
        }

        public Vector3 HexToWorldPos(Hex hex) {
            var cell = BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex);
            return tilemapLayers[BaseTilemapLayer].CellToWorld(cell);
        }

        public Task RemoveTile(Hex hex) {
            if (!Board.HasPosition(hex)) {
                return Task.CompletedTask;
            }
            // notify that tile is being removed before removing it
            externalBoardEventManager.Raise(ExternalBoardEvents.OnRemoveTile, new OnTileModifyEventArgs(hex));
            lock (BoardLock) {
                Board.RemovePosition(hex);
                tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
                RecalculateTiles();
                // outlines and fills are done separately
                tilemapLayers[BoardConstants.OutlineTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
            }
            return Task.CompletedTask;
        }

        public Task AddTile(Hex hex) {
            var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return Task.CompletedTask;
            }
            lock (BoardLock) {
                Board.AddPosition(hex);
                tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileBases[trait.Value]);
                tilemapLayers[BoardConstants.OutlineTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileOutlines[trait.Value]);
            }
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex));
            return Task.CompletedTask;
        }

            public void RecalculateTiles() {
            tilemapLayers[BaseTilemapLayer].RefreshAllTiles();
            tilemapLayers[BaseTilemapLayer].CompressBounds();
        }

        public TileBase GetTraitTileBase(ETrait trait) {
            return traitTileBases[trait];
        }

        #endregion
    }
    
    [Serializable]
    internal class TraitTiles : UDictionary<ETrait, TileBase> { }
}