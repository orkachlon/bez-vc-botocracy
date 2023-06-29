using Events.General;
using StoryPoints;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tutorial.StoryPoints {
    public class MTutorialStoryPoint : MStoryPoint {

        protected override void HandleStoryTurn() {
            base.HandleStoryTurn();
            if (Evaluated) {
                gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState); 
            }
        }
    }
}