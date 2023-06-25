using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audio;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using UnityEngine;

namespace Neurons.UI {
    public class MUIExpandNeuron : MUIBoardNeuron {

        [Header("Expand Neuron")]
        [SerializeField] private List<SpriteRenderer> blobs;

        [Header("Animation"), SerializeField] private AnimationCurve blobEasing;
        [SerializeField] private float blobHoverDuration;
        [SerializeField] private float blobAddDuration;
        [SerializeField] private float elasticity;
        [SerializeField] private float elasticDuration;


        private Sequence _hoverAnimation;
        
        private SExpandNeuronData ExpandData => RuntimeData.DataProvider as SExpandNeuronData;

        
        public override void ToHoverLayer() {
            base.ToHoverLayer();
            neuronFace.sortingOrder++;
            blobs.ForEach(b => {
                b.sortingLayerName = hoverSortingLayer;
                b.sortingOrder = neuronFace.sortingOrder - 1;
            });
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder++;
            blobs.ForEach(b => {
                b.sortingLayerName = belowConnSortingLayer;
                b.sortingOrder = SpriteRenderer.sortingOrder + 1;
            });
        }
        
        public override async Task PlayAddAnimation() {
            blobs.ForEach(s => s.transform.localScale = Vector3.zero);
            
            var blobAnimations = blobs
                .Select(blob => DOTween.Sequence()
                    .Append(blob.transform
                        .DOScale(Vector3.one, blobAddDuration)
                        .SetEase(Ease.OutElastic, elasticity, elasticDuration))
                    .AsyncWaitForCompletion())
                .ToList();
            await base.PlayAddAnimation();
            // await Task.Delay(100);
            await Task.WhenAll(blobAnimations);
            Task.WhenAll(blobs.Select(b => b.transform.DOScale(Vector3.zero, blobAddDuration).AsyncWaitForCompletion()));
        }
        
        public override Task PlayHoverAnimation() {
            if (_hoverAnimation != null && _hoverAnimation.IsPlaying()) {
                StopHoverAnimation();
            }
            _hoverAnimation = DOTween.Sequence();
            foreach (var blob in blobs) {
                InsertBlobHoverAnimation(blob.transform);
            }

            _hoverAnimation.SetAutoKill(false);
            _hoverAnimation.OnComplete(() => _hoverAnimation.Restart());
            return Task.CompletedTask;
        }

        public override void StopHoverAnimation() {
            _hoverAnimation?.Complete();
            _hoverAnimation?.Kill();
            _hoverAnimation = null;
        }

        public void PlaySpawnSound() {
            var source = AudioSpawner.GetAudioSource();
            source.Source.volume = addVolume;
            source.Source.pitch += (Random.value - 0.5f) * 0.5f;
            source.Source.PlayOneShot(ExpandData.GetSpawnAudio());
            AudioSpawner.ReleaseWhenDone(source);
        }

        public override void Default() {
            base.Default();
            ToBoardLayer();
            blobs.ForEach(b => { b.gameObject.SetActive(true); b.transform.localScale = Vector3.zero; });
            _hoverAnimation = null;
        }

        private void InsertBlobHoverAnimation(Transform blob) {
            var seq = DOTween.Sequence().Append(blob.DOScale(1, blobHoverDuration * 3).SetEase(blobEasing))
                .Append(blob.DOScale(0, blobHoverDuration * 2).SetEase(Ease.InQuad))
                .AppendInterval(blobHoverDuration * 2);
            _hoverAnimation.Insert(0, seq);
        }
    }
}