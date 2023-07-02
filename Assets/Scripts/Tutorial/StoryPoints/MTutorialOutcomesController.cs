using System;
using System.Threading.Tasks;
using DG.Tweening;
using Events.SP;
using StoryPoints.Outcomes;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.StoryPoints {
    public class MTutorialOutcomesController : MOutcomesController {

        private Image _bg;


        protected override void Awake() {
            base.Awake();
            _bg = GetComponent<Image>();
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
            var hidePos = new Vector2(_bg.rectTransform.anchoredPosition.x, -_bg.rectTransform.sizeDelta.y - 50); // added 50 for the expand button
            if (immediate) {
                _bg.rectTransform.anchoredPosition = hidePos;
                return;
            }
            await _bg.rectTransform.DOAnchorPosY(hidePos.y, 0.5f).AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false) {
            var showPos = new Vector2(_bg.rectTransform.anchoredPosition.x, 50);
            if (immediate) {
                _bg.rectTransform.anchoredPosition = showPos;
                return;
            }
            await _bg.rectTransform.DOAnchorPosY(showPos.y, 0.5f).AsyncWaitForCompletion();
        }

    }
}