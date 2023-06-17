using System;
using System.Linq;
using Core.EventSystem;
using Core.Utils.DataStructures;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem.Interfaces;
using MyHexBoardSystem.Events;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<IBoardNeuron>, INeuronBoardController {
        
        [Header("Tilemap"), SerializeField] private TraitTiles traitTileBases;
        [SerializeField] private TraitTiles pressedTileBases;
        [SerializeField] private TraitTiles traitTileOutlines;

        [Header("Event Managers"), SerializeField]
        private SEventManager externalBoardEventManager;

        protected override void CollectExistingTiles() {
            base.CollectExistingTiles();

            foreach (var hex in GetHexPoints()) {
                var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
                if (!trait.HasValue) {
                    continue;
                }
                tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileBases[trait.Value]);
            }
        }
        
        #region UnityMethods

        protected override void Start() {
            base.Start();
            externalBoardEventManager.Raise(ExternalBoardEvents.OnBoardSetupComplete, EventArgs.Empty);
        }

        private void OnEnable() {
            externalBoardEventManager.Register(ExternalBoardEvents.OnAddElement, OnTilePressed);
            externalBoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, OnTileUnpressed);
            externalBoardEventManager.Register(ExternalBoardEvents.OnMoveElement, OnNeuronMoved);
        }

        private void OnDisable() {
            externalBoardEventManager.Unregister(ExternalBoardEvents.OnAddElement, OnTilePressed);
            externalBoardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, OnTileUnpressed);
            externalBoardEventManager.Unregister(ExternalBoardEvents.OnMoveElement, OnNeuronMoved);
        }

        #endregion

        #region InterfaceMethods

        public int GetTraitTileCount(ETrait trait) {
            return Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait)).Count(h => Board.HasPosition(h));
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

        public void RemoveTile(Hex hex) {
            if (!Board.HasPosition(hex)) {
                return;
            }
            // notify that tile is being removed before removing it
            externalBoardEventManager.Raise(ExternalBoardEvents.OnRemoveTile, new OnTileModifyEventArgs(hex));
            Board.RemovePosition(hex);
            tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
            tilemapLayers[BaseTilemapLayer].RefreshAllTiles();
            tilemapLayers[BaseTilemapLayer].CompressBounds();
            // outlines and fills are done separately
            tilemapLayers[BoardConstants.OutlineTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
        }

        public void AddTile(Hex hex) {
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return;
            }

            Board.AddPosition(hex);
            tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileBases[trait.Value]);
            tilemapLayers[BoardConstants.OutlineTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTileOutlines[trait.Value]);
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex));
        }

        public TileBase GetTraitTileBase(ETrait trait) {
            return traitTileBases[trait];
        }

        #endregion

        #region EventHandlers

        private void OnTilePressed(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronData) {
                return;
            }

            PressTile(neuronData.Hex);
        }

        private void OnTileUnpressed(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronData) {
                return;
            }

            UnpressTile(neuronData.Hex);
        }

        private void OnNeuronMoved(EventArgs obj) {
            if (obj is not BoardElementMovedEventArgs<IBoardNeuron> moveArgs) {
                return;
            }
            UnpressTile(moveArgs.Hex);
            PressTile(moveArgs.To);
        }

        #endregion

        private void PressTile(Hex hex) {
            if (!Board.HasPosition(hex)) {
                return;
            }

            var dir = Manipulator.GetDirection(hex);
            if (!dir.HasValue) {
                return;
            }

            var trait = ITraitAccessor.DirectionToTrait(dir.Value);
            if (!trait.HasValue) {
                return;
            }

            SetTile(hex, pressedTileBases[trait.Value]);
        }

        private void UnpressTile(Hex hex) {
            if (!Board.HasPosition(hex)) {
                return;
            }

            var dir = Manipulator.GetDirection(hex);
            if (!dir.HasValue) {
                return;
            }

            var trait = ITraitAccessor.DirectionToTrait(dir.Value);
            if (!trait.HasValue) {
                return;
            }

            SetTile(hex, traitTileBases[trait.Value]);
        }
    }
    
    [Serializable]
    internal class TraitTiles : UDictionary<ETrait, TileBase> { }
}