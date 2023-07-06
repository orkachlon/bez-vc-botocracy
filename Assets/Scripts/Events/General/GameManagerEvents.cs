using System;
using Types.GameState;

namespace Events.General {
    public static class GameManagerEvents {
        public const string OnAfterGameStateChanged = "GM_OnAfterGameStateChanged";
        public const string OnBeforeGameStateChanged = "GM_OnBeforeGameStateChanged";
        public const string OnGameLoopStart = "GM_OnGameLoopStart";
    }

    public class GameStateEventArgs : EventArgs {
        public EGameState State;
        public EventArgs CustomArgs;

        public GameStateEventArgs(EGameState state, EventArgs customArgs = null) {
            State = state;
            CustomArgs = customArgs;
        }
    }

    public class LoseGameEventArgs : GameStateEventArgs {
        public ELoseReason LoseReason;
        
        public LoseGameEventArgs(ELoseReason reason, EventArgs customArgs = null) : base(EGameState.Lose, customArgs) {
            LoseReason = reason;
        }
    }

    public enum ELoseReason {
        NoMoreNeurons,
        BoardFull,
        TraitOutOfTiles,
        FromSP
    }
}