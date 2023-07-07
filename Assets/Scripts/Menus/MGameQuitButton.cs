using Types.Menus;
using UnityEngine;

namespace Menus {
    public class MGameQuitButton : MonoBehaviour, IClickableButton {

        public void OnButtonClick() {
            Application.Quit();
        }
    }
}