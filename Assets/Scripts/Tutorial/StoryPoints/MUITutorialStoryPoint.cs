using System;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using Events.Tutorial;
using StoryPoints.UI;
using Types.StoryPoint;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.StoryPoints {
    public class MUITutorialStoryPoint : MUIStoryPoint, IUIStoryPoint {

        [SerializeField] private SEventManager tutorialEventManager;
        
        private bool IsPauseEnabled { get; set; }


        protected override void OnEnable() {
            base.OnEnable();
            tutorialEventManager.Register(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }

        protected override void OnDisable() {
            base.OnDisable();
            tutorialEventManager.Unregister(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }
        
        public override async void CloseDecisionPopup() {
            await backGround.rectTransform.DOAnchorPosY(-backGround.rectTransform.sizeDelta.y, animationDuration).SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(SP));
            SP.Destroy();
        }

        protected override void PauseHide(EventArgs args) {
            if (IsPauseEnabled) {
                base.PauseHide(args);
            }
        }

        protected override void PauseShow(EventArgs args) {
            if (IsPauseEnabled) {
                base.PauseShow(args);
            }
        }

        private void UpdatePauseState(EventArgs obj) {
            if (obj is not TutorialStageEventArgs tutArgs) {
                return;
            }

            IsPauseEnabled = tutArgs.Stage switch {
                ETutorialStage.Introduction => false,
                ETutorialStage.NeuronRewards => false,
                ETutorialStage.Personalities => false,
                ETutorialStage.BoardEffects => true,
                ETutorialStage.Decisions => true,
                ETutorialStage.NeuronTypeIntro => true,
                ETutorialStage.ExpanderType => false,
                ETutorialStage.TravellerType => false,
                ETutorialStage.TimerType => false,
                ETutorialStage.CullerType => false,
                ETutorialStage.End => false,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}