using System;
using Core.EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Managers {
    public class MEventManagerSceneBinder : MonoBehaviour {
        
        [Header("Event Managers"), SerializeField] private SEventManager[] eventManagers;

        private const string OnGameReloaded = "GM_OnGameReloaded";

        private void OnEnable() {
            // SceneManager.sceneLoaded += ResetAllEventManagers;
            BindAllEventManagersToScene();
        }

        private void OnDisable() {
            // SceneManager.sceneLoaded -= ResetAllEventManagers;
            UnbindAllEventManagersFromScene();
        }

        private void Update() {
            if (Input.GetKey(KeyCode.R)) {
                ResetAllEventManagers();
                SceneManager.LoadScene("Level");
            }
        }

        private void BindAllEventManagersToScene() {
            foreach (var eventManager in eventManagers) {
                eventManager.BindToScene(OnGameReloaded);
            }
        }
        
        private void UnbindAllEventManagersFromScene() {
            foreach (var eventManager in eventManagers) {
                eventManager.BindToScene(OnGameReloaded);
            }
        }

        private void ResetAllEventManagers() {
            foreach (var eventManager in eventManagers) {
                eventManager.Raise(OnGameReloaded, EventArgs.Empty);
            }
        }
        
        private void ResetAllEventManagers(Scene scene, LoadSceneMode loadSceneMode) {
            foreach (var eventManager in eventManagers) {
                eventManager.Raise(OnGameReloaded, EventArgs.Empty);
            }
        }
    }
}