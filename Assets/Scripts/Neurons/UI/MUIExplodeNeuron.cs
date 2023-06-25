using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audio;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using UnityEngine;

namespace Neurons.UI {
    public class MUIExplodeNeuron : MUIBoardNeuron {

        [Header("Spikes"), SerializeField] private List<SpriteRenderer> spikes;
        [SerializeField] private AnimationCurve spikeSpawnEasing;
        [SerializeField] private float spikeSpawnDuration;

        private Sequence _hoverAnimation;
        
        private SExplodeNeuronData ExplodeData => RuntimeData.DataProvider as SExplodeNeuronData;
        

        public override void ToHoverLayer() {
            base.ToHoverLayer();
            neuronFace.sortingOrder++;
            spikes.ForEach(s => {
                s.sortingLayerName = hoverSortingLayer;
                s.sortingOrder = neuronFace.sortingOrder - 1;
            });
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder++;
            spikes.ForEach(s => {
                s.sortingLayerName = belowConnSortingLayer;
                s.sortingOrder = SpriteRenderer.sortingOrder + 1;
            });
        }
        
        public override async Task PlayAddAnimation() {
            spikes.ForEach(s => s.transform.localScale = Vector3.zero);
            var spikesAnimations = spikes
                .Select(spike => spike.transform
                    .DOScale(Vector3.one, spikeSpawnDuration)
                    .SetEase(spikeSpawnEasing)
                    .AsyncWaitForCompletion())
                .ToList();
            await base.PlayAddAnimation();
            await Task.Delay(50);
            await Task.WhenAll(spikesAnimations);
            Task.WhenAll(spikes.Select(s => s.transform.DOScale(Vector3.zero, spikeSpawnDuration * 2).SetDelay(spikeSpawnDuration).AsyncWaitForCompletion()));
        }
        
        public override Task PlayHoverAnimation() {
            if (_hoverAnimation != null && _hoverAnimation.IsPlaying()) {
                StopHoverAnimation();
            }
            _hoverAnimation = DOTween.Sequence(this);
            foreach (var spike in spikes) {
                InsertSpikeHoverAnimation(spike.transform);
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

        public void PlayKillSound() {
            var source = AudioSpawner.GetAudioSource();
            source.Source.pitch += (Random.value - 0.5f) * 0.5f;
            source.Source.PlayOneShot(ExplodeData.GetKillSound());
            AudioSpawner.ReleaseWhenDone(source);
        }

        private void InsertSpikeHoverAnimation(Transform spike) {
            var seq = DOTween.Sequence(this).Append(spike.DOScale(1, spikeSpawnDuration).SetEase(spikeSpawnEasing))
                .AppendInterval(spikeSpawnDuration * 2)
                .Append(spike.DOScale(0, spikeSpawnDuration * 3));
            _hoverAnimation.Insert(0, seq);
        }

        public override void Default() {
            base.Default();
            ToBoardLayer();
            spikes.ForEach(s => { s.gameObject.SetActive(true); s.transform.localScale = Vector3.zero; });
            _hoverAnimation = null;
        }
    }
}