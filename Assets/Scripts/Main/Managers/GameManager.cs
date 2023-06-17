using System;
using Core.EventSystem;
using Events.General;
using Events.SP;
using MyHexBoardSystem.Events;
using Neurons;
using Types.GameState;
using UnityEngine;

namespace Main.Managers {
    public class GameManager : MonoBehaviour {

        private EGameState _currentState;

        [Header("Event Managers"), SerializeField] private SEventManager gmEventManager;
        [SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;
        
        private void OnEnable() {
            // game state loop:
            // initGrid > playerTurn > eventTurn( > evaluation > outcome) > statTurn > playerTurn ...
            gmEventManager.Register(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Register(StoryEvents.OnStoryTurn, PlayerTurn);
            
            // end of game
            boardEventManager.Register(ExternalBoardEvents.OnBoardFull, Lose);
            boardEventManager.Register(ExternalBoardEvents.OnTraitOutOfTiles, Lose);
            neuronEventManager.Register(NeuronEvents.OnNoMoreNeurons, Lose);
            storyEventManager.Register(StoryEvents.OnNoMoreStoryPoints, Win);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Unregister(StoryEvents.OnStoryTurn, PlayerTurn);
            
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardFull, Lose);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitOutOfTiles, Lose);
            neuronEventManager.Unregister(NeuronEvents.OnNoMoreNeurons, Lose);
            storyEventManager.Unregister(StoryEvents.OnNoMoreStoryPoints, Win);
        }

        #region EventHandlers

        private void ChangeState(EGameState newState, EventArgs customArgs = null) {
            _currentState = newState;
            gmEventManager.Raise(GameManagerEvents.OnBeforeGameStateChanged, EventArgs.Empty);
            
            switch (newState) {
                case EGameState.StoryTurn:
                    break;
                case EGameState.PlayerTurn:
                    break;
                case EGameState.Win:
                    print("You win!");
                    break;
                case EGameState.Lose:
                    print("You lose!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            gmEventManager.Raise(GameManagerEvents.OnAfterGameStateChanged, new GameStateEventArgs(_currentState, customArgs));
            
        }

        #endregion

        private void Lose(EventArgs customArgs) {
            ChangeState(EGameState.Lose, customArgs);
        }

        private void Win(EventArgs customArgs) {
            ChangeState(EGameState.Win, customArgs);
        }

        private void PlayerTurn(EventArgs customArgs) {
            ChangeState(EGameState.PlayerTurn, customArgs);
        }
        
        private void StoryTurn(EventArgs customArgs) {
            ChangeState(EGameState.StoryTurn, customArgs);
        }
    }
}
