using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;
using UnityEngine;
using System.Linq;
using Core.Utils.DataStructures;
using ExternBoardSystem.BoardSystem.Board;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<BoardNeuron>, INeuronBoardController {

        [Header("Event Managers"), SerializeField]
        private SEventManager externalBoardEventManager;

        [Header("Tilemap"), SerializeField] private TraitTiles traitTiles;


        #region UnityMethods

        protected override void Start() {
            base.Start();
            externalBoardEventManager.Raise(ExternalBoardEvents.OnBoardSetupComplete, EventArgs.Empty);
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
        }

        public void AddTile(Hex hex) {
            Board.AddPosition(hex);
            // todo figure out which tile should be added using the direction
            // tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), traitTiles[]);
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex));
        }

        #endregion
    }
    
    [Serializable]
    internal class TraitTiles : UDictionary<ETrait, TileBase> { }
}