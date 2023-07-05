using System;
using Core.EventSystem;
using Core.Utils.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Managers {
    public class MEventManagerSceneBinder : MSingleton<MEventManagerSceneBinder> {
        
        [Header("Event Managers"), SerializeField] private SEventManager[] eventManagers;

        private const string OnGameReloaded = "GM_OnGameReloaded";

        private void OnEnable() {
            // SceneManager.sceneLoaded += ResetAllEventManagers;
            //SceneManager.activeSceneChanged += ResetAllEventManagers;
            //SceneManager.sceneUnloaded += ResetAllEventManagers;
            BindAllEventManagersToScene();
        }

        private void OnDisable() {
            // SceneManager.sceneLoaded -= ResetAllEventManagers;
            //SceneManager.activeSceneChanged -= ResetAllEventManagers;
            //SceneManager.sceneUnloaded -= ResetAllEventManagers;
            UnbindAllEventManagersFromScene();
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.R)) {
                return;
            }
            ResetAllEventManagers();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void BindAllEventManagersToScene() {
            foreach (var eventManager in eventManagers) {
                eventManager.BindToScene(OnGameReloaded);
            }
        }
        
        private void UnbindAllEventManagersFromScene() {
            foreach (var eventManager in eventManagers) {
                eventManager.UnbindFromScene(OnGameReloaded);
            }
        }
        
        private void ResetAllEventManagers(Scene current) {
            foreach (var eventManager in eventManagers) {
                eventManager.Raise(OnGameReloaded, EventArgs.Empty);
            }
        }

        public static void ResetAllEventManagers() {
            foreach (var eventManager in Instance.eventManagers) {
                eventManager.Raise(OnGameReloaded, EventArgs.Empty);
            }
        }
    }
}