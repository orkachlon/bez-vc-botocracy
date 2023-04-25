using System;
using Tiles;
using Traits;
using UnityEngine;
using Utils;
using Grid = Grids.Grid;

namespace Managers {
    public class GameManager : MonoBehaviour {

        private GameState _currentState;

        public static event Action<GameState> OnGameStateChanged;
        
        private void Awake() {
            // todo should these be methods and unsubscribe on destroy?
            Grid.GridInitDone += () => ChangeState(GameState.EventTurn);
            GameEventManager.OnNextEvent += () => ChangeState(GameState.PlayerTurn);
            NeuronManager.OnNeuronPlaced += () => ChangeState(GameState.EventEvaluation);
            GameEventManager.OnEventEvaluated += () => ChangeState(GameState.StatEvaluation);
            GameEventManager.OnNoMoreEvents += () => ChangeState(GameState.StatEvaluation);
            StatManager.OnStatsEvaluated += isGameLost => ChangeState(isGameLost ? GameState.Lose : GameEventManager.Instance.HasEvents() ? GameState.PlayerTurn : GameState.Win);
        }

        private void Start() {
            ChangeState(GameState.InitGrid);
        }

        private void ChangeState(GameState newState) {
            _currentState = newState;
            
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
                    foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                        Debug.Log($"{trait} -> {Grid.Instance.CountNeurons(trait)}");
                    }
                    break;
                case GameState.Lose:
                    print("You lose!");
                    foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                        Debug.Log($"{trait} -> {Grid.Instance.CountNeurons(trait)}");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            OnGameStateChanged?.Invoke(_currentState);
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
