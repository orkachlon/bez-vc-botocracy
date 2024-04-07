using System;
using Core.EventSystem;
using Core.EventSystem.EventBus;
using Events.Board;
using Events.EventBindings;
using Events.General;
using Events.Neuron;
using Events.SP;
using Types.GameState;
using UnityEngine;

namespace Main.Managers {
    public class GameManager : MonoBehaviour {

        private EGameState _currentState;

        [Header("Event Managers"), SerializeField] protected SEventManager gmEventManager;
        [SerializeField] protected SEventManager neuronEventManager;
        [SerializeField] protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager boardEventManager;

        private EventBinding<OnNoMoreStoryPoints> noMoreStoryPointsEB;
        
        protected virtual void OnEnable() {
            // game state loop:
            // initGrid > playerTurn > eventTurn( > evaluation > outcome) > statTurn > playerTurn ...
            gmEventManager.Register(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Register(StoryEvents.OnStoryTurn, PlayerTurn);
            
            // end of game
            boardEventManager.Register(ExternalBoardEvents.OnBoardFull, LoseBoardFull);
            boardEventManager.Register(ExternalBoardEvents.OnTraitOutOfTiles, LostTraitNoTiles);
            boardEventManager.Register(ExternalBoardEvents.OnAllTraitsOutOfTiles, LostFromSP);
            neuronEventManager.Register(NeuronEvents.OnNoMoreNeurons, LoseNoNeurons);
            //storyEventManager.Register(StoryEvents.OnNoMoreStoryPoints, Win);

            noMoreStoryPointsEB = new EventBinding<OnNoMoreStoryPoints>(Win);
            EventBus<OnNoMoreStoryPoints>.Register(noMoreStoryPointsEB);
        }

        protected virtual void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnGameLoopStart, PlayerTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, StoryTurn);
            storyEventManager.Unregister(StoryEvents.OnStoryTurn, PlayerTurn);
            
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardFull, LoseBoardFull);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitOutOfTiles, LostTraitNoTiles);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllTraitsOutOfTiles, LostFromSP);
            neuronEventManager.Unregister(NeuronEvents.OnNoMoreNeurons, LoseNoNeurons);
            //storyEventManager.Unregister(StoryEvents.OnNoMoreStoryPoints, Win);

            EventBus<OnNoMoreStoryPoints>.Unregister(noMoreStoryPointsEB);
        }

        #region EventHandlers

        private void ChangeState(EGameState newState, EventArgs customArgs = null) {
            _currentState = newState;
            gmEventManager.Raise(GameManagerEvents.OnBeforeGameStateChanged, EventArgs.Empty);
            
            switch (newState) {
                case EGameState.StoryTurn:
                    break;
                case EGameState.PlayerTurn:
                    break;
                case EGameState.Win:
                    print("You win!");
                    break;
                case EGameState.Lose:
                    print("You lose!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            gmEventManager.Raise(GameManagerEvents.OnAfterGameStateChanged, new GameStateEventArgs(_currentState, customArgs));
            
        }

        #endregion

        private void LostFromSP(EventArgs customArgs) {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.FromSP, customArgs));
        }

        private void LostTraitNoTiles(EventArgs customArgs) {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.TraitOutOfTiles, customArgs));
        }

        private void LoseNoNeurons(EventArgs customArgs) {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.NoMoreNeurons, customArgs));
        }
        
        private void LoseBoardFull(EventArgs customArgs) {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.BoardFull, customArgs));
        }

        private void Win(EventArgs customArgs) {
            ChangeState(EGameState.Win, customArgs);
        }

        private void Win(OnNoMoreStoryPoints onNoMoreStoryPoints) {
            ChangeState(EGameState.Win);
        }

        protected void PlayerTurn(EventArgs customArgs) {
            if (_currentState is EGameState.Win or EGameState.Lose) {
                return;
            }
            ChangeState(EGameState.PlayerTurn, customArgs);
        }

        protected void StoryTurn(EventArgs customArgs) {
            if (_currentState is EGameState.Win or EGameState.Lose) {
                return;
            }
            ChangeState(EGameState.StoryTurn, customArgs);
        }

        [ContextMenu("Win Game")]
        private void WinGameNow() {
            ChangeState(EGameState.Win);
        }
        
        [ContextMenu("Lose game full board")]
        private void LoseGameFullBoardNow() {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.BoardFull));
        }
        
        [ContextMenu("Lose game trait no tiles")]
        private void LoseGameTraitOutOfTilesNow() {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.TraitOutOfTiles));
        }
        
        [ContextMenu("Lose game no neurons")]
        private void LoseGameNoNeuronsNow() {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.NoMoreNeurons));
        }
        
        [ContextMenu("Lose game from SP")]
        private void LoseGameFromSP() {
            ChangeState(EGameState.Lose, new LoseGameEventArgs(ELoseReason.FromSP));
        }
    }
}
