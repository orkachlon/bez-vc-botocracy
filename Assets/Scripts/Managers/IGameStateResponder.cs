namespace Managers {
    public interface IGameStateResponder {
        // void HandleBeforeGameStateChanged(GameManager.GameState state);
        void HandleAfterGameStateChanged(GameManager.GameState state);
    }
}