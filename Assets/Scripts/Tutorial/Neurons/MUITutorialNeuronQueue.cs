using Core.EventSystem;
using Events.Tutorial;
using Neurons.NeuronQueue;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tutorial.Neurons {
    public class MUITutorialNeuronQueue : MUINeuronQueue {

        [SerializeField] private SEventManager tutorialEventManager;

        [SerializeField] private Image bg;



        protected override void OnEnable() {
            base.OnEnable();
            tutorialEventManager.Register(TutorialEvents.OnHideNeuronQueue, Hide);
            tutorialEventManager.Register(TutorialEvents.OnShowNeuronQueue, Show);
        }


        protected override void OnDisable() {
            base.OnDisable();
            tutorialEventManager.Unregister(TutorialEvents.OnHideNeuronQueue, Hide);
            tutorialEventManager.Unregister(TutorialEvents.OnShowNeuronQueue, Show);
        }

        private void Hide(EventArgs args = null) {
            bg.rectTransform.anchoredPosition = new Vector2(-bg.rectTransform.sizeDelta.x, bg.rectTransform.anchoredPosition.y);
        }

        private void Show(EventArgs args = null) {
            bg.rectTransform.anchoredPosition = new Vector2(100, bg.rectTransform.anchoredPosition.y);
        }
    }
}