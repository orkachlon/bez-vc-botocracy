using Animation;
using Core.Utils;
using Events.Board;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardSystem;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.Traits;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;

namespace Assets.Scripts.Tutorial.Board {
    public class MTutorialBoardModifier : MBoardModifier {

        private MTutorialTraitAccessor TutorialTraitAccessor => base.TraitAccessor as MTutorialTraitAccessor;


        public override async Task AddTilesToTrait(ETrait trait, int amount) {
            for (int i = 0; i < amount; i++) {
                await AddTileToTrait(trait, TutorialAddTileSelector, 50);
            }
        }

        protected override async Task RemoveTileFromTrait(ETrait trait, int delay = 0) {
            var edgeHexes = TutorialTraitAccessor.TraitToDirections(trait)
                .SelectMany(d => BoardController.Manipulator.GetEdge(d))
                .ToArray();
            if (edgeHexes.Length == 0) {
                return;
            }
            Hex chosenHex;
            if (edgeHexes.All(h => BoardController.Manipulator.GetNeighbours(h).Length <= 1)) {
                chosenHex = edgeHexes[Random.Range(0, edgeHexes.Length)]; 
            } else {
                var hexesWithMoreThan1Neighbor = edgeHexes.Where(h => BoardController.Manipulator.GetNeighbours(h).Length > 1).ToArray();
                chosenHex = hexesWithMoreThan1Neighbor[Random.Range(0, hexesWithMoreThan1Neighbor.Length)];
            }
            await AnimationManager.Register(RemoveTile(chosenHex, delay));
        }

        public Hex TutorialAddTileSelector(INeuronBoardController controller, ETrait trait) {
            var edgeHexes = BoardController.Manipulator.GetEdge();
            if (edgeHexes.Length == 0) {
                return TutorialTraitAccessor.TraitToDirection(trait);
            }
            var surroundingHexes = BoardController.Manipulator.GetSurroundingHexes(edgeHexes, true);
            var onlyEmptySurroundingHexes = surroundingHexes.Where(h => !BoardController.Board.HasPosition(h));
            var onlyContainedInTrait = onlyEmptySurroundingHexes.Where(h =>
                    TutorialTraitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(h)) == trait)
                .ToArray();
            // give priority to tiles within a smaller radius from center
            var orderedByRadius = onlyContainedInTrait
                 .OrderBy(h => h.Length)
                 .ToArray();
            return orderedByRadius[0];
        }
    }
}