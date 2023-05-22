using System;

namespace Main.Managers {
    public interface IGameStateResponder {
        // void HandleBeforeGameStateChanged(GameManager.GameState state);
        void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null);
    }
}