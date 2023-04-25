namespace Managers {
    public interface IGameStateResponder {
        void HandleGameStateChanged(GameManager.GameState state);
    }
}