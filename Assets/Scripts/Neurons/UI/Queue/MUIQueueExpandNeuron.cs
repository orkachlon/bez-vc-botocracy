using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Neurons.UI.Queue {
    public class MUIQueueExpandNeuron : MUIQueueNeuron {

        [Header("Expand Neuron")]
        [SerializeField] private List<Image> blobs;

        [Header("Animation"), SerializeField] private AnimationCurve blobEasing;
        [SerializeField] private float blobDuration;

        private Sequence _animation;

        private SExpandNeuronData ExpandData => RuntimeData.DataProvider as SExpandNeuronData;

        protected override void UpdateView() {
            base.UpdateView();
            if (RuntimeData.PlaceInQueue > 2) {
                blobs.ForEach(b => {
                    b.color = Color.white;
                    b.gameObject.SetActive(false);
                });
            }
        }

        public override async Task PlayAnimation() {
            if (_animation != null && _animation.IsPlaying()) {
                return;
            }
            await Task.Delay((int) Random.value * 500);
            _animation = DOTween.Sequence();
            foreach (var blob in blobs) {
                blob.gameObject.SetActive(true);
                InsertBlobAnimation(blob.transform);
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
            var blobSeq = DOTween.Sequence();
            foreach (var blob in blobs) {
                blobSeq.Insert(0, blob.DOFade(0, fadeDuration));
            }
            StopAnimation();
            await Task.WhenAll(base.AnimateDequeue(), blobSeq.AsyncWaitForCompletion());
        }

        public override Task AnimateQueueShift(int queueIndex, int stackShiftAmount, int Top3ShiftAmount) {
            if (queueIndex <= 2) {
                blobs.ForEach(b => {
                    b.gameObject.SetActive(true);
                    b.transform.localScale = Vector3.one;
                });
            }
            return base.AnimateQueueShift(queueIndex, stackShiftAmount, Top3ShiftAmount);
        }

        private void InsertBlobAnimation(Transform blob) {
            blob.transform.localScale = Vector3.zero;
            var seq = DOTween.Sequence().Append(blob.DOScale(1, blobDuration * 3).SetEase(blobEasing))
                .Append(blob.DOScale(0, blobDuration * 2).SetEase(Ease.InQuad))
                .AppendInterval(blobDuration * 2);
            _animation.Insert(0, seq);
        }
    }
}