using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.StoryPoints;
using Main.Traits;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<BoardNeuron>, INeuronBoardController {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;


        #region UnityMethods

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        #endregion

        #region InterfaceMethods

        public int GetTraitTileCount(ETraitType trait) {
            return Manipulator.GetTriangle(INeuronBoardController.TraitToDirection(trait)).Count(h => Board.HasPosition(h));
        }

        public void SetColor(Hex[] hexTiles, Color color) {
            foreach (var hexTile in hexTiles) {
                SetColor(hexTile, color);
            }
        }

        public void SetColor(Hex hexTile, Color color) {
            var offsetCoord = OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hexTile);
            tilemap.RemoveTileFlags(offsetCoord.ToVector3Int(), TileFlags.LockColor);
            tilemap.SetColor(offsetCoord.ToVector3Int(), color);
        }

        public Color GetColor(Hex tile) {
            return tilemap.GetColor(OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, tile).ToVector3Int());
        }

        #endregion

        #region EventHandlers

        private void OnBoardEffect(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }
            // todo add effect on board (remove tiles?)
            print(storyEventArgs.Story.Description);
        }

        #endregion
    }
}