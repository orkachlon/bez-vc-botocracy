using MyHexBoardSystem.BoardSystem;
using System.Collections.Generic;
using System.Linq;
using Tutorial.Traits;
using Types.Hex.Coordinates;
using Types.Trait;
using Types.Tutorial;

namespace Tutorial.Board {
    public class MTutorialBoardController : MNeuronBoardController, ITutorialBoardController {


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

        public void DisableHexes(IEnumerable<Hex> hexes = null) {
            hexes ??= Board.Positions.Select(p => p.Point).ToArray();
            foreach (var hex in hexes) {
                if (Board.HasPosition(hex)) {
                    Board.GetPosition(hex).IsEnabled = false;
                }
            }
        }

        public void EnableHexes(IEnumerable<Hex> hexes = null) {
            hexes ??= Board.Positions.Select(p => p.Point).ToArray();
            foreach (var hex in hexes) {
                if (Board.HasPosition(hex)) {
                    Board.GetPosition(hex).IsEnabled = true;
                }
            }
        }
    }
}