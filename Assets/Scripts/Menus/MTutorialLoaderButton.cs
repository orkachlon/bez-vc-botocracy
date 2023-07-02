using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class MTutorialLoaderButton : MMainMenuBaseButton, IMenuButton {
        
        [SerializeField] private string sceneName;
        
        public void OnButtonClick() {
            SceneManager.LoadScene(sceneName);
        }
    }
}