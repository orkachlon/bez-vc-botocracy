using System.Threading.Tasks;
using Events.General;
using StoryPoints;

namespace Tutorial.StoryPoints {
    public class MTutorialStoryPoint : MStoryPoint {

        public bool IsSPEnabled { get; set; }
        
        private MUITutorialStoryPoint _tutUISP => UISP as MUITutorialStoryPoint;

        protected override void HandleStoryTurn() {
            if (!IsSPEnabled) {
                return;
            }
            base.HandleStoryTurn();
            if (Evaluated) {
                gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState); 
            }
        }

        public Task AwaitHideAnimation() {
            _tutUISP.Hide();
            return Task.CompletedTask;
        }
    }
}