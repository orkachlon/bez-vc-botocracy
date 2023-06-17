using System;
using Types.GameState;

namespace Main.Managers {
    public interface IGameStateResponder {
        // void HandleBeforeGameStateChanged(GameManager.EGameState state);
        void HandleAfterGameStateChanged(EGameState state, EventArgs customArgs = null);
    }
}