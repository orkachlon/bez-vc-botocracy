using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            neuronFace.sortingOrder = hoverSortingOrder + 2;
            spikes.ForEach(s => s.sortingOrder = hoverSortingOrder + 1);
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder = boardSortingOrder + 3;
            spikes.ForEach(s => s.sortingOrder = boardSortingOrder);
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
            Task.WhenAll(spikes.Select(b => b.transform.DOScale(Vector3.zero, spikeSpawnDuration * 2).AsyncWaitForCompletion()));

        }
        
        public override Task PlayHoverAnimation() {
            if (_hoverAnimation != null && _hoverAnimation.IsPlaying()) {
                StopHoverAnimation();
            }
            _hoverAnimation = DOTween.Sequence();
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
        }

        public void PlayKillSound() {
            Source.PlayOneShot(ExplodeData.GetKillSound());
        }

        private void InsertSpikeHoverAnimation(Transform spike) {
            var seq = DOTween.Sequence().Append(spike.DOScale(1, spikeSpawnDuration).SetEase(spikeSpawnEasing))
                .AppendInterval(spikeSpawnDuration * 2)
                .Append(spike.DOScale(0, spikeSpawnDuration * 3));
            _hoverAnimation.Insert(0, seq);
        }

        public override void Default() {
            base.Default();
            ToBoardLayer();
            spikes.ForEach(s => { s.gameObject.SetActive(true); s.transform.localScale = Vector3.zero; });
            _hoverAnimation = null;
            neuronFace.sprite = null;
        }
    }
}