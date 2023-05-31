using System;
using Core.EventSystem;
using TMPro;
using UnityEngine;

namespace Main.StoryPoints {
    public class MUIStoryPoint : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private TextMeshProUGUI rewardText;

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
            description.text = story.Description;
            UpdateRewardAmount(story.Reward);
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
            turnCounter.text = $"Turns: {turns}";
        }

        private void UpdateRewardAmount(int reward) {
            rewardText.text = $"Reward: {reward}";
        }
    }
}