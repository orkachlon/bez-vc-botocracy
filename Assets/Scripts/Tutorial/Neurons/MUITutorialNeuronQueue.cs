using Neurons.NeuronQueue;
using System;
using Core.EventSystem;
using Events.Tutorial;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.Neurons {
    public class MUITutorialNeuronQueue : MUINeuronQueue {

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

        protected override void Hide(EventArgs args) {
            if (IsPauseEnabled) {
                base.Hide(args);
            }
        }

        protected override void Show(EventArgs args) {
            if (IsPauseEnabled) {
                base.Show(args);
            }
        }

        private void UpdatePauseState(EventArgs obj) {
            if (obj is not TutorialStageEventArgs tutArgs) {
                return;
            }

            IsPauseEnabled = tutArgs.Stage switch {
                ETutorialStage.Introduction => false,
                ETutorialStage.NeuronRewards => true,
                ETutorialStage.Personalities => true,
                ETutorialStage.BoardEffects => true,
                ETutorialStage.Decisions => true,
                ETutorialStage.NeuronTypeIntro => true,
                ETutorialStage.ExpanderType => true,
                ETutorialStage.TravellerType => true,
                ETutorialStage.TimerType => true,
                ETutorialStage.CullerType => true,
                ETutorialStage.End => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}