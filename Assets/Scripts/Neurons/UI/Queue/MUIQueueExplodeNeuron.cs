using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Neurons.UI.Queue {
    public class MUIQueueExplodeNeuron : MUIQueueNeuron {

        [Header("Spikes"), SerializeField] private List<Image> spikes;
        [SerializeField] private AnimationCurve spikeEasing;
        [SerializeField] private float spikeDuration;

        private Sequence _animation;

        private SExplodeNeuronData ExplodeData => RuntimeData.DataProvider as SExplodeNeuronData;


        protected override void UpdateView() {
            base.UpdateView();
            if (RuntimeData.PlaceInQueue > 2) {
                spikes.ForEach(s => {
                    s.color = Color.white;
                    s.transform.localScale = Vector3.zero;
                    s.gameObject.SetActive(false);
                });
            }
        }

        public override async Task PlayAnimation() {
            if (_animation != null && _animation.IsPlaying()) {
                return;
            }
            await Task.Delay((int) Random.value * 500);
            _animation = DOTween.Sequence();
            foreach (var spike in spikes) {
                spike.gameObject.SetActive(true);
                spike.transform.localScale = Vector3.zero;
                InsertSpikeHoverAnimation(spike.transform);
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
            var spikeSeq = DOTween.Sequence();
            foreach (var spike in spikes) {
                spikeSeq.Insert(0, spike.DOFade(0, fadeDuration));
            }
            StopAnimation();
            await Task.WhenAll(base.AnimateDequeue(), spikeSeq.AsyncWaitForCompletion());
        }

        private void InsertSpikeHoverAnimation(Transform spike) {
            var seq = DOTween.Sequence(this).Append(spike.DOScale(1, spikeDuration).SetEase(spikeEasing))
                .AppendInterval(spikeDuration * 2)
                .Append(spike.DOScale(0, spikeDuration * 3))
                .AppendInterval(spikeDuration * 2);
            _animation.Insert(0, seq);
        }
    }
}