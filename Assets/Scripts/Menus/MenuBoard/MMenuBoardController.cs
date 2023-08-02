using Core.EventSystem;
using Events.Board;
using Events.Menu;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Menus.MenuBoard {
    public class MMenuBoardController : MNeuronBoardController {

        [SerializeField] private float delayBetweenTiles;
        [SerializeField] private bool animate;
        [SerializeField] private SEventManager menuEventManager;

        protected override void Start() {
            base.Start();
            if (!animate) {
                menuEventManager.Raise(MenuEvents.OnBoardAnimated, EventArgs.Empty);
                return;
            }
            // hide tiles and animate into frame
            foreach (var tile in GetHexPoints()) {
                RemoveTile(tile);
            }
            StartCoroutine(AnimateAddAllTiles());
        }

        private IEnumerator AnimateAddAllTiles() {
            foreach (var tile in GetHexPoints().OrderByDescending(t => t.Length)) {
                AddTile(tile);
                yield return new WaitForSeconds(delayBetweenTiles);
            }
            menuEventManager.Raise(MenuEvents.OnBoardAnimated, EventArgs.Empty);
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
            externalBoardEventManager.Raise(ExternalBoardEvents.OnAddTile, new OnTileModifyEventArgs(hex, 0.3f));
            return Task.CompletedTask;
        }

        public override Task RemoveTile(Hex hex) {
            var tilemap = GetTilemap(BaseTilemapLayer);
            if (!Board.HasPosition(hex) || tilemap == null) {
                return Task.CompletedTask;
            }
            // notify that tile is being removed before removing it
            externalBoardEventManager.Raise(ExternalBoardEvents.OnRemoveTile, new OnTileModifyEventArgs(hex, 0));
            lock (BoardLock) {
                Board.RemovePosition(hex);
                tilemap.SetTile(BoardManipulationOddR<BoardNeuron>.GetCellCoordinate(hex), null);
                RecalculateTiles();
            }
            return Task.CompletedTask;
        }
    }
}