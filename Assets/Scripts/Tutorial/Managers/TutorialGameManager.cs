using Events.Board;
using Events.General;
using Events.SP;
using Main.Managers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tutorial.Managers {
    public class TutorialGameManager : GameManager {

        protected override void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Register(StoryEvents.OnStoryTurn, PlayerTurn);
        }

        protected override void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Unregister(StoryEvents.OnStoryTurn, PlayerTurn);
        }
    }
}