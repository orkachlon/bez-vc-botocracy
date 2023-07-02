using Main.Managers;
using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPauseButton : MonoBehaviour, IMenuButton {

    [SerializeField] private string sceneName;
    
    public void OnButtonClick() {
        MEventManagerSceneBinder.ResetAllEventManagers();
        SceneManager.LoadScene(sceneName);
    }
}
