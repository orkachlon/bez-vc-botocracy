using System;
using Core.EventSystem;
using Events.General;
using Events.UI;
using Types.GameState;
using Types.Menus;
using UnityEngine;

namespace Menus.PauseMenu {
    public class MPauseButton : MonoBehaviour, IClickableButton {

        [SerializeField] private SEventManager uiEventManager;
        [SerializeField] private SEventManager gmEventManager;

        private bool _isPaused;
        private EGameState _gameState;
    
        public void OnButtonClick() {
            if (_gameState is EGameState.Win or EGameState.Lose) {
                return;
            }
            uiEventManager.Raise(_isPaused ? UIEvents.OnGameUnpaused : UIEvents.OnGamePaused, new PauseArgs(!_isPaused));
        }

        private void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, UpdateGameState);
            uiEventManager.Register(UIEvents.OnGamePaused, UpdateState);
            uiEventManager.Register(UIEvents.OnGameUnpaused, UpdateState);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, UpdateState);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, UpdateState);
        }

        private void UpdateGameState(EventArgs obj) {
            if (obj is not GameStateEventArgs stateEventArgs) {
                return;
            }

            _gameState = stateEventArgs.State;
        }

        private void UpdateState(EventArgs obj) {
            if (obj is not PauseArgs pauseArgs) {
                return;
            }

            _isPaused = pauseArgs.IsPaused;
        }
        
        
    }
}
