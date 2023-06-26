using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Neurons.UI.Queue {
    public class MUIQueueDecayNeuron : MUIQueueNeuron {
        [Header("Dots"), SerializeField] private List<Image> dots;
        [SerializeField] private float dotDuration;

        private Sequence _animation;

        private SDecayingNeuronData DecayData => RuntimeData.DataProvider as SDecayingNeuronData;


        protected override void UpdateView() {
            base.UpdateView();
            if (RuntimeData.PlaceInQueue > 2) {
                dots.ForEach(d => {
                    d.color = Color.white;
                    d.gameObject.SetActive(false);
                }); 
            }
        }

        #region Animation

        public override async Task PlayAnimation() {
            if (_animation != null && _animation.IsPlaying()) {
                return;
            }
            await Task.Delay((int) Random.value * 500);
            _animation = DOTween.Sequence();
            foreach (var dot in dots) {
                dot.gameObject.SetActive(true);
                dot.transform.localScale = Vector3.one;
                _animation.Append(dot.transform.DOScale(0, dotDuration * 2));
            }

            foreach (var dot in dots) {
                _animation.Append(dot.transform.DOScale(1, dotDuration * 2));
            }

            _animation.SetAutoKill(false);
            _animation.SetLoops(-1, LoopType.Restart);
        }

        public override void StopAnimation() {
            _animation?.Complete();
            _animation?.Kill();
            _animation = null;
        }

        public override async Task AnimateDequeue() {
            var dotSeq = DOTween.Sequence();
            foreach(var dot in dots) {
                dotSeq.Insert(0, dot.DOFade(0, fadeDuration));
            }
            StopAnimation();
            await Task.WhenAll(base.AnimateDequeue(), dotSeq.AsyncWaitForCompletion());
        }

        public override Task AnimateQueueShift(int queueIndex, int stackShiftAmount, int Top3ShiftAmount) {
            if (queueIndex <= 2) {
                dots.ForEach(d => {
                    d.gameObject.SetActive(true);
                    d.transform.localScale = Vector3.one;
                });
            }
            return base.AnimateQueueShift(queueIndex, stackShiftAmount, Top3ShiftAmount);
        }

        #endregion
    }
}