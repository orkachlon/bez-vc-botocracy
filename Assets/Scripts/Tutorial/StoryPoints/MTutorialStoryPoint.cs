using Events.General;
using StoryPoints;

namespace Assets.Scripts.Tutorial.StoryPoints {
    public class MTutorialStoryPoint : MStoryPoint {

        public bool IsSPEnabled { get; set; }

        protected override void HandleStoryTurn() {
            if (!IsSPEnabled) {
                return;
            }
            base.HandleStoryTurn();
            if (Evaluated) {
                gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState); 
            }
        }
    }
}