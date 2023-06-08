using System;
using Core.EventSystem;
using TMPro;
using UnityEngine;

namespace Main.StoryPoints.UI {
    public class MUIStoryPointCounter : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private TextMeshProUGUI _storyPointCounterText;
        private int _spCount;

        #region UnityMethods

        private void Awake() {
            _storyPointCounterText = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, UpdateStoryCounter);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, UpdateStoryCounter);
        }

        #endregion

        private void UpdateStoryCounter(EventArgs obj) {
            if (obj is not StoryEventArgs) {
                return;
            }

            _spCount++;
            UpdateText();
        }

        private void UpdateText() {
            _storyPointCounterText.text = $"Survived for {_spCount} year{(_spCount != 1 ? "s" : "")}";
        }
    }
}