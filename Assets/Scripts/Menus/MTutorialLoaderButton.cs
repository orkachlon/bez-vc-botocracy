﻿using Core.EventSystem;
using Types.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class MTutorialLoaderButton : MonoBehaviour, IClickableButton {
        
        [SerializeField] private string sceneName;
        
        public void OnButtonClick() {
            MEventManagerSceneBinder.ResetAllEventManagers();
            SceneManager.LoadScene(sceneName);
        }
    }
}