using System;

namespace Managers {
    public static class GameManagerEvents {
        public const string OnAfterGameStateChanged = "GMOnAfterGameStateChanged";
    }

    public class GameStateEventArgs : EventArgs {
        public GameManager.GameState State;

        public GameStateEventArgs(GameManager.GameState state) {
            State = state;
        }
    }
}