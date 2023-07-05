using System;
using Core.EventSystem;
using Events.SP;
using Events.Tutorial;
using StoryPoints.Outcomes;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.StoryPoints {
    public class MTutorialOutcomesController : MOutcomesController {

        [SerializeField] private SEventManager tutorialEventManager;
        
        private bool IsPauseEnabled { get; set; }

        protected override void OnEnable() {
            base.OnEnable();
            storyEventManager.Register(StoryEvents.OnEvaluate, ShowPanel);
            tutorialEventManager.Register(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }

        protected override void OnDisable() {
            base.OnDisable();
            storyEventManager.Unregister(StoryEvents.OnEvaluate, ShowPanel);
            tutorialEventManager.Unregister(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }

        protected override void PauseHide(EventArgs args) {
            if (IsPauseEnabled && Count > 0) {
                base.PauseHide(args);
            }
        }

        protected override void PauseShow(EventArgs args) {
            if (IsPauseEnabled && Count > 0) {
                base.PauseShow(args);
            }
        }

        private async void ShowPanel(EventArgs args) {
            await Show();
        }
        
        private void UpdatePauseState(EventArgs obj) {
            if (obj is not TutorialStageEventArgs tutArgs) {
                return;
            }

            IsPauseEnabled = tutArgs.Stage switch {
                ETutorialStage.Introduction => false,
                ETutorialStage.NeuronRewards => false,
                ETutorialStage.Personalities => false,
                ETutorialStage.BoardEffects => false,
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