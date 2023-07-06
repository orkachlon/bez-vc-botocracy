using Types.Menus;
using UnityEngine;

namespace Menus {
    public class MGameQuitButton : MonoBehaviour, IMenuButton {

        public void OnButtonClick() {
            Application.Quit();
        }
    }
}