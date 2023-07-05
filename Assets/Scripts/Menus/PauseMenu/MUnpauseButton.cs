using System;
using Core.EventSystem;
using Events.UI;
using Types.Menus;
using UnityEngine;

namespace Main.UI.PauseMenu {
    public class MUnpauseButton : MPaseMenuButtonBase, IMenuButton {

        [SerializeField] private SEventManager uiEventManager;
        
        public void OnButtonClick() {
            uiEventManager.Raise(UIEvents.OnGameUnpaused, new PauseArgs(false));
        }
    }
}