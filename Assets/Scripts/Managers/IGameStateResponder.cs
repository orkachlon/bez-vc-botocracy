using System;

namespace Managers {
    public interface IGameStateResponder {
        // void HandleBeforeGameStateChanged(GameManager.GameState state);
        void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null);
    }
}