using System;
using Core.EventSystem;
using ExternBoardSystem.Events;
using Main.GameStats;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using Main.StoryPoints;
using UnityEngine;

namespace Main.Managers {
    public class GameManager : MonoBehaviour {

        private GameState _currentState;

        [Header("Event Managers"), SerializeField] private SEventManager gmEventManager;
        [SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager statEventManager;
        [SerializeField] private SEventManager boardEventManager;
        
        private void Awake() {
            // game state loop:
            // initGrid > playerTurn > eventTurn( > evaluation > outcome) > statTurn > playerTurn ...
            // todo should these be methods and unsubscribe on destroy?
            gmEventManager.Register(GameManagerEvents.OnGameLoopStart, e => ChangeState(GameState.PlayerTurn, e));
            neuronEventManager.Register(NeuronEvents.OnNeuronPlaced, e => ChangeState(GameState.BoardStateBroadcast, e));
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, e => ChangeState(GameState.StoryTurn, e));
            neuronEventManager.Register(NeuronEvents.OnNoMoreNeurons, e => ChangeState(GameState.Lose, e));
            storyEventManager.Register(StoryEvents.OnStoryTurn, e => ChangeState(GameState.StatTurn, e));
            storyEventManager.Register(StoryEvents.OnNoMoreStoryPoints, e => ChangeState(GameState.Win, e));
            statEventManager.Register(StatEvents.OnStatOutOfBounds, e => ChangeState(GameState.Lose, e));
            statEventManager.Register(StatEvents.OnStatTurn, e => ChangeState(GameState.PlayerTurn, e));
        }

        private void Start() {
            ChangeState(GameState.InitGrid);
        }

        #region EventHandlers

        private void ChangeState(GameState newState, EventArgs customArgs = null) {
            _currentState = newState;
            gmEventManager.Raise(GameManagerEvents.OnBeforeGameStateChanged, EventArgs.Empty);
            
            switch (newState) {
                case GameState.InitGrid:
                    break;
                case GameState.StoryTurn:
                    break;
                case GameState.PlayerTurn:
                    break;
                case GameState.BoardStateBroadcast:
                    break;
                case GameState.EventEvaluation:
                    break;
                case GameState.StatTurn:
                    break;
                case GameState.Win:
                    print("You win!");
                    statEventManager.Raise(StatEvents.OnPrintStats, EventArgs.Empty);
                    break;
                case GameState.Lose:
                    print("You lose!");
                    statEventManager.Raise(StatEvents.OnPrintStats, EventArgs.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            gmEventManager.Raise(GameManagerEvents.OnAfterGameStateChanged, new GameStateEventArgs(_currentState, customArgs));
            
        }

        #endregion
    }

    public enum GameState {
        InitGrid,
        StoryTurn,
        PlayerTurn,
        EventEvaluation,
        StatTurn,
        Win,
        Lose,
        BoardStateBroadcast
    }
}
