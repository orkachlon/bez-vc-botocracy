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
        [SerializeField] private AnimationCurve dotSpawnEasing;
        [SerializeField] private float dotSpawnDuration;
        [SerializeField] private int dotScaleCorrection = 320;

        private Sequence _animation;

        private SDecayingNeuronData DecayData => RuntimeData.DataProvider as SDecayingNeuronData;


        protected override void UpdateView() {
            base.UpdateView();
            if (RuntimeData.PlaceInQueue > 2) {
                dots.ForEach(d => d.gameObject.SetActive(false)); 
            }
        }

        #region Animation

        public override Task PlayAnimation() {
            if (_animation != null && _animation.IsPlaying()) {
                StopAnimation();
            }
            _animation = DOTween.Sequence();
            dots.ForEach(d => d.gameObject.SetActive(true));
            foreach (var dot in dots) {
                _animation.Append(dot.transform.DOScale(0, dotSpawnDuration * 2));
            }

            foreach (var dot in dots) {
                _animation.Append(dot.transform.DOScale(dotScaleCorrection, dotSpawnDuration * 2));
            }

            _animation.SetAutoKill(false);
            _animation.OnComplete(() => _animation.Restart());
            return Task.CompletedTask;
        }

        public override void StopAnimation() {
            _animation?.Complete();
            _animation?.Kill();
        }

        public override async Task AnimateDequeue() {
            var dotSeq = DOTween.Sequence();
            foreach(var dot in dots) {
                dotSeq.Insert(0, dot.DOFade(0, fadeDuration));
            }
            StopAnimation();
            await Task.WhenAll(base.AnimateDequeue(), dotSeq.AsyncWaitForCompletion());
        }

        #endregion
    }
}