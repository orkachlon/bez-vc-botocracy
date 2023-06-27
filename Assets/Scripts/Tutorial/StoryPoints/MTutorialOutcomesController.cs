using Core.EventSystem;
using Events.Tutorial;
using StoryPoints.Outcomes;
using System;
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
            tutorialEventManager.Register(TutorialEvents.OnHideOutcomes, Hide);
            tutorialEventManager.Register(TutorialEvents.OnShowOutcomes, Show);
        }


        protected override void OnDisable() {
            base.OnDisable();
            tutorialEventManager.Unregister(TutorialEvents.OnHideOutcomes, Hide);
            tutorialEventManager.Unregister(TutorialEvents.OnShowOutcomes, Show);
        }

        private void Hide(EventArgs args = null) {
            bg.rectTransform.anchoredPosition = new Vector2 (bg.rectTransform.sizeDelta.x, bg.rectTransform.anchoredPosition.y);
        }

        private void Show(EventArgs args = null) {
            bg.rectTransform.anchoredPosition = new Vector2(-100, bg.rectTransform.anchoredPosition.y);
        }

    }
}