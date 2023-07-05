using Core.EventSystem;
using Events.UI;
using Types.Menus;
using UnityEngine;

namespace Menus.PauseMenu {
    public class MUnpauseButton : MPauseMenuButtonBase, IMenuButton {

        [SerializeField] private SEventManager uiEventManager;
        
        public void OnButtonClick() {
            uiEventManager.Raise(UIEvents.OnGameUnpaused, new PauseArgs(false));
        }
    }
}