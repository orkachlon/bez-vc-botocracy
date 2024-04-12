using Core.Utils.Singleton;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scenes {
    public class MSceneLoader : MSingleton<MSceneLoader> {

        public static event Action<string> OnSceneLoad;
        public static event Action<string> OnSceneUnload;

        private void OnEnable() {
            OnSceneLoad += SetActiveScene;
        }

        private void OnDisable() {
            OnSceneLoad -= SetActiveScene;
        }

        public static void ReplaceScene(Scene current, string next) {
            SceneManager.UnloadSceneAsync(current);
            Instance.StartCoroutine(LoadScene(next));
        }

        private static void SetActiveScene(string name) {
            var newScene = SceneManager.GetSceneByName(name);
            SceneManager.SetActiveScene(newScene);
        }

        private static IEnumerator LoadScene(string name) {
            var asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            while (!asyncLoad.isDone) {
                Debug.Log("Loading the Scene");
                yield return null;
            }

            OnSceneLoad?.Invoke(name);
        }
    }
}
