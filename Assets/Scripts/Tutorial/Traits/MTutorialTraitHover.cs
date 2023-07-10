using System.Linq;
using MyHexBoardSystem.UI.TraitHover;
using Types.Trait;

namespace Tutorial.Traits {
    public class MTutorialTraitHover : MTraitHover {
        protected override void CacheHoverData(ETrait hoverTrait) {
            CurrentHighlightedTrait = hoverTrait;
            var affectedTraits = CurrentSP.DecidingTraits[hoverTrait].BoardEffect;
            foreach (var trait in affectedTraits.Keys.Where(t => TutorialConstants.Traits.Contains(t))) {
                if (affectedTraits[trait] > 0) {
                    CurrentPositive.Add(trait);
                }
                else if (affectedTraits[trait] < 0) {
                    CurrentNegative.Add(trait);
                }
            }

        }
    }
}