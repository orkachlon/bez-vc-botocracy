using Core.EventSystem;
using Events.Tutorial;
using MyHexBoardSystem.BoardSystem;
using System.Collections.Generic;
using System.Linq;
using Tutorial.Traits;
using Types.Hex.Coordinates;
using Types.Trait;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.Board {
    public class MTutorialBoardController : MNeuronBoardController, ITutorialBoardController {

        [SerializeField] private SEventManager tutorialEventManager;

        private MTutorialTraitAccessor TutorialTraitAccessor => _traitAccessor as MTutorialTraitAccessor;

        #region InterfaceMethods

        public override int GetTraitTileCount(ETrait trait) {
            if (!TutorialConstants.Traits.Contains(trait)) {
                return 0;
            }
            var directions = TutorialTraitAccessor.TraitToDirections(trait);
            return directions.SelectMany(d => Manipulator.GetTriangle(d)).Count(h => Board.HasPosition(h));
        }

        #endregion

        public void DisableHexes(Hex[] hexes = null, bool immediate = false) {
            hexes ??= Board.Positions.Select(p => p.Point).ToArray();
            foreach (var hex in hexes) {
                if (Board.HasPosition(hex)) {
                    Board.GetPosition(hex).IsEnabled = false;
                }
            }
            tutorialEventManager.Raise(TutorialEvents.OnTilesDisabled, new TutorialTilesEventArgs(hexes, false, immediate));
        }

        public void EnableHexes(Hex[] hexes = null, bool immediate = false) {
            hexes ??= Board.Positions.Select(p => p.Point).ToArray();
            foreach (var hex in hexes) {
                if (Board.HasPosition(hex)) {
                    Board.GetPosition(hex).IsEnabled = true;
                }
            }
            tutorialEventManager.Raise(TutorialEvents.OnTilesEnabled, new TutorialTilesEventArgs(hexes, true, immediate));
        }
    }
}