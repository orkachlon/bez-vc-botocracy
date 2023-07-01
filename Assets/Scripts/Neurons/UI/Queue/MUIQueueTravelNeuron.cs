using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Neurons.UI.Queue {
    public class MUIQueueTravelNeuron : MUIQueueNeuron {

        [Header("Probes"), SerializeField] private List<Image> probes;
        [SerializeField] private AnimationCurve probeEasing;
        [SerializeField] private float probeDuration;
        [SerializeField] private List<Image> lines;

        private Sequence _animationSequence;
        private Coroutine _animationCoroutine;

        private STravelNeuronData TravelData => RuntimeData.DataProvider as STravelNeuronData;

        protected override void UpdateView() {
            base.UpdateView();
            probes.ForEach(p => {
                p.color = Color.white;
                p.gameObject.SetActive(RuntimeData.PlaceInQueue <= 2);
            });
            lines.ForEach(l => {
                l.color = Color.white;
                l.gameObject.SetActive(RuntimeData.PlaceInQueue <= 2);
            });
        }

        #region Animation

        public override Task PlayAnimation() {
            if (_animationCoroutine != null) {
                return Task.CompletedTask;
            }
            _animationCoroutine = StartCoroutine(QueueAnimation());
            return Task.CompletedTask;
        }

        public override void StopAnimation() {
            if (_animationCoroutine != null) {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            KillQueueAnimationSequence();
        }

        public override async Task AnimateDequeue() {
            var probeSeq = DOTween.Sequence();
            foreach (var probe in probes) {
                probeSeq.Insert(0, probe.DOFade(0, fadeDuration));
            }
            foreach (var line in lines) {
                probeSeq.Insert(0, line.DOFade(0, fadeDuration));
            }
            StopAnimation();
            await Task.WhenAll(base.AnimateDequeue(), probeSeq.AsyncWaitForCompletion());
        }

        public override Task AnimateQueueShift(int queueIndex, int stackShiftAmount, int top3ShiftAmount) {
            if (queueIndex <= 2) {
                probes.ForEach(p => {
                    p.gameObject.SetActive(true);
                    p.transform.localScale = Vector3.one;
                });
                lines.ForEach(l => {
                    l.gameObject.SetActive(true);
                    l.transform.localScale = Vector3.one;
                });
            }
            return base.AnimateQueueShift(queueIndex, stackShiftAmount, top3ShiftAmount);
        }

        private IEnumerator QueueAnimation() {
            while (true) {
                if (_animationSequence != null) {
                    KillQueueAnimationSequence();
                }

                _animationSequence = DOTween.Sequence(this);
                var randomAngle = 60 * Random.Range(1, 4) * (Random.value > 0.5f ? 1 : -1);
                for (var i = 0; i < probes.Count; i++) {
                    var probe = probes[i];
                    var line = lines[i];
                    probe.gameObject.SetActive(true);
                    line.gameObject.SetActive(true);
                    _animationSequence.Insert(0, probe.transform
                        .DORotate(Vector3.forward * randomAngle, probeDuration, RotateMode.LocalAxisAdd)
                        .SetEase(probeEasing));
                    _animationSequence.Insert(0, line.transform
                        .DORotate(Vector3.forward * randomAngle, probeDuration, RotateMode.LocalAxisAdd)
                        .SetEase(probeEasing));
                }

                _animationSequence.SetAutoKill(false);
                _animationSequence.AppendInterval(probeDuration);
                _animationSequence.Play();

                yield return new WaitWhile(_animationSequence.IsPlaying);
            }
        }

        private void KillQueueAnimationSequence() {
            _animationSequence?.Complete();
            _animationSequence?.Kill();
            _animationSequence = null;
        }

        #endregion
    }
}