using System;
using System.Linq;
using Traits;
using UnityEngine;
using Utils;
using Grid = Grids.Grid;

namespace Managers {
    public class GameManager : MonoBehaviour {

        private GameState _currentState;

        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;
        
        private void Awake() {
            // game state loop:
            // initGrid > eventTurn > playerTurn > eventEvaluation > statEvaluation > outcome > eventTurn > playerTurn ...
            // initGrid > playerTurn > eventTurn( > evaluation > outcome) > statTurn > playerTurn ...
            // todo should these be methods and unsubscribe on destroy?
            Grid.GridInitDone += () => ChangeState(GameState.PlayerTurn);
            NeuronManager.OnNeuronPlaced += () => ChangeState(GameState.EventTurn);
            GameEventManager.OnEventTurn += () => ChangeState(GameState.StatTurn);
            // GameEventManager.OnEventEvaluated += () => ChangeState(GameState.StatTurn);
            GameEventManager.OnNoMoreEvents += () => ChangeState(GameState.StatTurn);
            StatManager.OnStatTurn += isGameLost => ChangeState(isGameLost ? GameState.Lose : GameEventManager.Instance.HasEvents() ? GameState.PlayerTurn : GameState.Win);
            NeuronManager.Instance.OnNoMoreNeurons += () => ChangeState(GameState.Lose);
        }

        private void Start() {
            ChangeState(GameState.InitGrid);
        }

        private void ChangeState(GameState newState) {
            _currentState = newState;
            OnBeforeGameStateChanged?.Invoke(newState);
            
            switch (newState) {
                case GameState.InitGrid:
                    break;
                case GameState.EventTurn:
                    break;
                case GameState.PlayerTurn:
                    break;
                case GameState.EventEvaluation:
                    break;
                case GameState.StatTurn:
                    break;
                case GameState.Win:
                    print("You win!");
                    StatManager.Instance.PrintStats();
                    break;
                case GameState.Lose:
                    print("You lose!");
                    StatManager.Instance.PrintStats();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            OnAfterGameStateChanged?.Invoke(_currentState);
        }

        public enum GameState {
            InitGrid,
            EventTurn,
            PlayerTurn,
            EventEvaluation,
            StatTurn,
            Win,
            Lose
        }
    }
}
