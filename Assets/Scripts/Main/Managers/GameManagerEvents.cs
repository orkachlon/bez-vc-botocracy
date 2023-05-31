using System;

namespace Main.Managers {
    public static class GameManagerEvents {
        public const string OnAfterGameStateChanged = "GMOnAfterGameStateChanged";
        public const string OnBeforeGameStateChanged = "GMOnBeforeGameStateChanged";
        public const string OnGameLoopStart = "GMOnGameLoopStart";
    }

    public class GameStateEventArgs : EventArgs {
        public GameState State;
        public EventArgs CustomArgs;

        public GameStateEventArgs(GameState state, EventArgs customArgs = null) {
            State = state;
            CustomArgs = customArgs;
        }
    }
}