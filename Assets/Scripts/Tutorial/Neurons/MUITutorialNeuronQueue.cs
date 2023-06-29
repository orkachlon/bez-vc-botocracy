using Core.EventSystem;
using DG.Tweening;
using Events.Tutorial;
using Neurons.NeuronQueue;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tutorial.Neurons {
    public class MUITutorialNeuronQueue : MUINeuronQueue {

        [SerializeField] private SEventManager tutorialEventManager;

        [SerializeField] private Image bg;


        public async Task Hide(bool immediate = false, EventArgs args = null) {
            if (immediate) {
                bg.rectTransform.anchoredPosition = new Vector2(-bg.rectTransform.sizeDelta.x, bg.rectTransform.anchoredPosition.y);
                return;
            }
            await bg.rectTransform.DOAnchorPosX(-bg.rectTransform.sizeDelta.x, 0.5f).AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false,  EventArgs args = null) {
            if (immediate) {
                bg.rectTransform.anchoredPosition = new Vector2(100, bg.rectTransform.anchoredPosition.y);
                return;
            }
            await bg.rectTransform.DOAnchorPosX(100, 0.5f).AsyncWaitForCompletion();
        }
    }
}