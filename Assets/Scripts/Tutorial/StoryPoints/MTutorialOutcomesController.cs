using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using Events.Tutorial;
using StoryPoints.Outcomes;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tutorial.StoryPoints {
    public class MTutorialOutcomesController : MOutcomesController {

        [SerializeField] private SEventManager tutorialEventManager;
        
        
        private Image bg;


        protected override void Awake() {
            base.Awake();
            bg = GetComponent<Image>();
        }

        protected override void OnEnable() {
            base.OnEnable();
            storyEventManager.Register(StoryEvents.OnEvaluate, ShowPanel);
        }

        protected override void OnDisable() {
            base.OnDisable();
            storyEventManager.Unregister(StoryEvents.OnEvaluate, ShowPanel);
        }

        private async void ShowPanel(EventArgs args) {
            await Show();
        }

        public async Task Hide(bool immediate = false) {
            var hidePos = new Vector2(bg.rectTransform.anchoredPosition.x, -bg.rectTransform.sizeDelta.y - 50); // added 50 for the expand button
            if (immediate) {
                bg.rectTransform.anchoredPosition = hidePos;
                return;
            }
            await bg.rectTransform.DOAnchorPosY(hidePos.y, 0.5f).AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false) {
            var showPos = new Vector2(bg.rectTransform.anchoredPosition.x, 50);
            if (immediate) {
                bg.rectTransform.anchoredPosition = showPos;
                return;
            }
            await bg.rectTransform.DOAnchorPosY(showPos.y, 0.5f).AsyncWaitForCompletion();
        }

    }
}