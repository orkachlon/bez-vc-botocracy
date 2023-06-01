using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.StoryPoints;
using Main.Traits;
using UnityEngine;
using System.Linq;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<BoardNeuron>, INeuronBoardController {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager externalBoardEventManager;
        

        #region UnityMethods

        protected override void Start() {
            base.Start();
            externalBoardEventManager.Raise(ExternalBoardEvents.OnBoardSetupComplete, EventArgs.Empty);
        }

        #endregion

        #region InterfaceMethods

        public int GetTraitTileCount(ETraitType trait) {
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
            // notify that tile is being removed
            if (!Board.HasPosition(hex)) {
                return;
            }
            Board.RemovePosition(hex);
            tilemapLayers[BaseTilemapLayer].SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
            tilemapLayers[BaseTilemapLayer].RefreshAllTiles();
            tilemapLayers[BaseTilemapLayer].CompressBounds();
        }

        #endregion
    }
}