using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class MSceneLoaderButton : MonoBehaviour, IMenuButton {

        [SerializeField] private string sceneName;

        protected RectTransform RT;

        protected virtual void Awake() {
            RT = GetComponent<RectTransform>();
        }

        public void OnButtonClick() {
            SceneManager.LoadScene(sceneName);
        }

    }
}