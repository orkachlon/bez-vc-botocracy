using System;

namespace Main.Managers {
    public static class GameManagerEvents {
        public const string OnAfterGameStateChanged = "GM_OnAfterGameStateChanged";
        public const string OnBeforeGameStateChanged = "GM_OnBeforeGameStateChanged";
        public const string OnGameLoopStart = "GM_OnGameLoopStart";
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