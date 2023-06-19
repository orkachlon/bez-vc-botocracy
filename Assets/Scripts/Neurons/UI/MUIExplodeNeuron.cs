using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Types.Board;
using UnityEngine;

namespace Neurons.UI {
    public class MUIExplodeNeuron : MUIBoardNeuron {

        [Header("Spikes"), SerializeField] private List<SpriteRenderer> spikes;
        [SerializeField] private SpriteRenderer neuronFace;
        [SerializeField] private AnimationCurve spikeSpawnEasing;
        [SerializeField] private float spikeSpawnDuration;

        private Sequence _hoverAnimation;

        protected override void UpdateView() {
            spikes.ForEach(s => { s.gameObject.SetActive(true); s.transform.localScale = Vector3.one; });
        }
        
        public override void SetRuntimeElementData(IBoardElement data) {
            base.SetRuntimeElementData(data);
            _hoverAnimation = null;
        }
        
        public override void ToHoverLayer() {
            base.ToHoverLayer();
            neuronFace.sortingOrder = hoverSortingOrder + 2;
            spikes.ForEach(s => s.sortingOrder = hoverSortingOrder + 1);
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder = hoverSortingOrder + 3;
            spikes.ForEach(s => s.sortingOrder = boardSortingOrder + 2);
        }
        
        public override async Task PlayAddAnimation() {
            base.PlayAddAnimation();
            spikes.ForEach(s => s.transform.localScale = Vector3.zero);
            var spikesAnimations = spikes
                .Select(spike => spike.transform
                    .DOScale(Vector3.one, spikeSpawnDuration)
                    .SetEase(spikeSpawnEasing)
                    .AsyncWaitForCompletion())
                .ToList();
            await Task.WhenAll(spikesAnimations);
        }
        
        public override Task PlayHoverAnimation() {
            if (_hoverAnimation != null && _hoverAnimation.IsPlaying()) {
                StopHoverAnimation();
            }
            _hoverAnimation = DOTween.Sequence();
            foreach (var spike in spikes) {
                _hoverAnimation.Insert(0, spike.transform.DOScale(0, spikeSpawnDuration * 3));
            }

            var outSeq = DOTween.Sequence();
            foreach (var spike in spikes) {
                outSeq.Insert(0, spike.transform.DOScale(1, spikeSpawnDuration).SetEase(spikeSpawnEasing)).AppendInterval(spikeSpawnDuration);
            }

            _hoverAnimation.Append(outSeq);

            _hoverAnimation.SetAutoKill(false);
            _hoverAnimation.OnComplete(() => _hoverAnimation.Restart());
            return Task.CompletedTask;
        }
        
        public override void StopHoverAnimation() {
            _hoverAnimation?.Complete();
            _hoverAnimation?.Kill();
        }
    }
}