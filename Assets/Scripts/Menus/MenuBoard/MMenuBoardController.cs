using Events.Board;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;

namespace Menus.MenuBoard {
    public class MMenuBoardController : MNeuronBoardController {


        protected override void Start() {
            base.Start();
            // hide tiles and animate into frame
            foreach (var tile in GetHexPoints()) {
                RemoveTile(tile);
            }
            StartCoroutine(AnimateAddAllTiles());
        }

        protected override void CollectExistingTiles() {
            base.CollectExistingTiles();
        }

        private IEnumerator AnimateAddAllTiles() {
            foreach (var tile in GetHexPoints().OrderByDescending(t => t.Length)) {
                AddTile(tile);
                yield return null;
            }
        }

        public override Task AddTile(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue || tilemap == null) {
                return Task.CompletedTask;
            }
            lock (BoardLock) {
                Board.AddPosition(hex);
                tilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), GetTraitTileBase(trait.Value));
            }
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex));
            return Task.CompletedTask;
        }

        public override Task RemoveTile(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            if (!Board.HasPosition(hex) || tilemap == null) {
                return Task.CompletedTask;
            }
            // notify that tile is being removed before removing it
            externalBoardEventManager.Raise(ExternalBoardEvents.OnRemoveTile, new OnTileModifyEventArgs(hex));
            lock (BoardLock) {
                Board.RemovePosition(hex);
                tilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
                RecalculateTiles();
            }
            return Task.CompletedTask;
        }
    }
}