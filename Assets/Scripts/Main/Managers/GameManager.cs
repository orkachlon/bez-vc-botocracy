using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.StoryPoints;
using UnityEngine;

namespace Main.Managers {
    public class GameManager : MonoBehaviour {

        private GameState _currentState;

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

        private void ChangeState(GameState newState, EventArgs customArgs = null) {
            _currentState = newState;
            gmEventManager.Raise(GameManagerEvents.OnBeforeGameStateChanged, EventArgs.Empty);
            
            switch (newState) {
                case GameState.StoryTurn:
                    break;
                case GameState.PlayerTurn:
                    break;
                case GameState.Win:
                    print("You win!");
                    break;
                case GameState.Lose:
                    print("You lose!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            gmEventManager.Raise(GameManagerEvents.OnAfterGameStateChanged, new GameStateEventArgs(_currentState, customArgs));
            
        }

        #endregion

        private void Lose(EventArgs customArgs) {
            ChangeState(GameState.Lose, customArgs);
        }

        private void Win(EventArgs customArgs) {
            ChangeState(GameState.Win, customArgs);
        }

        private void PlayerTurn(EventArgs customArgs) {
            ChangeState(GameState.PlayerTurn, customArgs);
        }
        
        private void StoryTurn(EventArgs customArgs) {
            ChangeState(GameState.StoryTurn, customArgs);
        }
    }

    public enum GameState {
        StoryTurn,
        PlayerTurn,
        Win,
        Lose
    }
}
