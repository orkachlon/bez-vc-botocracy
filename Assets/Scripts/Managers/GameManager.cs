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
            // initGrid > eventTurn > playerTurn > eventEvaluation > statEvaluation > eventTurn > playerTurn ...
            // todo should these be methods and unsubscribe on destroy?
            Grid.GridInitDone += () => ChangeState(GameState.EventTurn);
            GameEventManager.OnNextEvent += () => ChangeState(GameState.PlayerTurn);
            NeuronManager.OnNeuronPlaced += () => ChangeState(GameState.EventEvaluation);
            GameEventManager.OnEventEvaluated += () => ChangeState(GameState.StatEvaluation);
            GameEventManager.OnNoMoreEvents += () => ChangeState(GameState.StatEvaluation);
            StatManager.OnStatsEvaluated += isGameLost => ChangeState(isGameLost ? GameState.Lose : GameEventManager.Instance.HasEvents() ? GameState.EventTurn : GameState.Win);
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
                case GameState.StatEvaluation:
                    break;
                case GameState.Win:
                    print("You win!");
                    PrintStats();
                    break;
                case GameState.Lose:
                    print("You lose!");
                    PrintStats();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            OnAfterGameStateChanged?.Invoke(_currentState);
        }

        private static void PrintStats() {
            var msg = string.Join("\n",
                EnumUtil.GetValues<ETraitType>()
                    .Select(trait => $"{trait} -> {Grid.Instance.CountNeurons(trait)}"));
            Debug.Log(msg);

            msg = string.Join("\n", StatManager.Instance.Select(stat => $"{stat} -> {stat.Value}"));
            Debug.Log(msg);
        }

        public enum GameState {
            InitGrid,
            EventTurn,
            PlayerTurn,
            EventEvaluation,
            StatEvaluation,
            Win,
            Lose
        }
    }
}
