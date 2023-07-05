using Menus.MainMenu;
using Types.Menus;
using UnityEngine;

namespace Menus {
    public class MGameQuitButton : MMainMenuBaseButton, IMenuButton {

        public void OnButtonClick() {
            Application.Quit();
        }
    }
}