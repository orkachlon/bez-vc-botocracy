using System;
using Core.EventSystem;
using Events.SP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.StoryPoints.UI {
    public class MUIStoryPoint : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private Image artwork;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Register(StoryEvents.OnDecrement, OnDecrementStory);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Unregister(StoryEvents.OnDecrement, OnDecrementStory);
        }

        #region EventHandlers

        private void OnInitStory(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            var story = storyEventArgs.Story;
            title.text = story.Title;
            description.text = story.Description;
            artwork.sprite = story.Artwork;
            UpdateTurnCounter(story.TurnsToEvaluation);
        }

        private void OnDecrementStory(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            UpdateTurnCounter(storyEventArgs.Story.TurnsToEvaluation);
        }

        #endregion

        private void UpdateTurnCounter(int turns) {
            turnCounter.text = $"{turns}";
        }
    }
}