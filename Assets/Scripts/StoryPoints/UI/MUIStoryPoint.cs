using System;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StoryPoints.UI {
    public class MUIStoryPoint : MonoBehaviour {
        [Header("Visuals"), SerializeField] private RectTransform backGround; 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private Image artwork;
        [SerializeField] private Image closeButton;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private RectTransform _rectTransform;

        #region UnityMethods

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Register(StoryEvents.OnDecrement, OnDecrementStory);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Unregister(StoryEvents.OnDecrement, OnDecrementStory);
        }

        #endregion

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

        public async Task PlayInitAnimation() {
            backGround.anchoredPosition = new Vector2(-backGround.sizeDelta.x, 50);
            await backGround.DOAnchorPosX(50, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        public async Task PlayRemoveAnimation() {
            await backGround.DOAnchorPosY(-backGround.sizeDelta.y, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        private void UpdateTurnCounter(int turns) {
            turnCounter.text = $"{turns}";
        }
    }
}