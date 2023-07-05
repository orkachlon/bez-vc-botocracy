using System;
using Core.EventSystem;
using Events.UI;
using Types.Menus;
using UnityEngine;

namespace Menus.PauseMenu {
    public class MPauseButton : MonoBehaviour, IMenuButton {

        [SerializeField] private SEventManager uiEventManager;

        private bool _isPaused;
    
        public void OnButtonClick() {
            uiEventManager.Raise(_isPaused ? UIEvents.OnGameUnpaused : UIEvents.OnGamePaused, new PauseArgs(!_isPaused));
        }

        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnGamePaused, UpdateState);
            uiEventManager.Register(UIEvents.OnGameUnpaused, UpdateState);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, UpdateState);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, UpdateState);
        }

        private void UpdateState(EventArgs obj) {
            if (obj is not PauseArgs pauseArgs) {
                return;
            }

            _isPaused = pauseArgs.IsPaused;
        }
        
        
    }
}
