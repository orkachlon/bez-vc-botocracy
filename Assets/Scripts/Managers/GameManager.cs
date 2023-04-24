using System;
using Tiles;
using UnityEngine;
using Grid = Grids.Grid;

namespace Managers {
    public class GameManager : MonoBehaviour {

        private GameState _currentState;

        public static event Action<GameState> OnGameStateChanged;
        
        private void Awake() {
            // todo should these be methods and unsubscribe on destroy?
            Grid.GridInitDone += () => ChangeState(GameState.EventTurn);
            GameEventManager.EventDone += () => ChangeState(GameState.PlayerTurn);
            Tile.OnTileClickedEvent += _ => ChangeState(GameState.EventTurn);
            GameEventManager.NoMoreEvents += () => ChangeState(GameState.Win);
        }

        private void Start() {
            ChangeState(GameState.InitGrid);
        }

        public void ChangeState(GameState newState) {
            _currentState = newState;
            
            switch (newState) {
                case GameState.InitGrid:
                    break;
                case GameState.EventTurn:
                    break;
                case GameState.PlayerTurn:
                    break;
                case GameState.Win:
                    print("You win!");
                    break;
                case GameState.Lose:
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
            Win,
            Lose
        }
    }
}
