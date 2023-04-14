using System;
using UnityEngine;
using Grid = Grids.Grid;

namespace Managers {
    public class GameManager : MonoBehaviour {
    
        private void Start() {
            ChangeState(GameState.CreateGrid);
        }

        public void ChangeState(GameState state) {
            switch (state) {
                case GameState.CreateGrid:
                    Grid.Instance.CreateGrid();
                    break;
                case GameState.RestOfGame:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public enum GameState {
            CreateGrid,
            RestOfGame
        }
    }
}
