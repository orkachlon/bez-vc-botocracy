using DG.Tweening;
using Neurons.NeuronQueue;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Tutorial.Neurons {
    public class MUITutorialNeuronQueue : MUINeuronQueue {
        
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