using Core.EventSystem;
using Core.Scenes;
using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class MLevelLoaderButton : MonoBehaviour, IClickableButton {

        [SerializeField] private string sceneName;
        
        public void OnButtonClick() {
            MEventManagerSceneBinder.ResetAllEventManagers();
            //SceneManager.LoadScene(sceneName);
            MSceneLoader.ReplaceScene(gameObject.scene, sceneName);
        }

    }
}