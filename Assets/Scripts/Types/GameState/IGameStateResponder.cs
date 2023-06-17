using System;

namespace Types.GameState {
    public interface IGameStateResponder {
        // void HandleBeforeGameStateChanged(GameManager.EGameState state);
        void HandleAfterGameStateChanged(EGameState state, EventArgs customArgs = null);
    }
}