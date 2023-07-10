using System;
using Core.EventSystem;
using Events.UI;
using UnityEngine;

namespace Menus.PauseMenu {
    
    // ~~~~~~~UNUSED
    public class MPauseTimeScaler : MonoBehaviour {
        
        [SerializeField] private SEventManager uiEventManager;

        
        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnGamePaused, FreezeTime);
            uiEventManager.Register(UIEvents.OnGameUnpaused, ReleaseTime);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, FreezeTime);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, ReleaseTime);
        }


        private void FreezeTime(EventArgs args) {
            Time.timeScale = 0;
        }

        private void ReleaseTime(EventArgs args) {
            Time.timeScale = 1;
        }
    }
}