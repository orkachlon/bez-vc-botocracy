using Assets.Scripts.Menus;
using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class MSceneLoaderButton : MMainMenuBaseButton, IMenuButton {

        [SerializeField] private string sceneName;

        public void OnButtonClick() {
            SceneManager.LoadScene(sceneName);
        }

    }
}