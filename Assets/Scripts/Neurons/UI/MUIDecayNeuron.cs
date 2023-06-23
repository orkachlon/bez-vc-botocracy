using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using UnityEngine;

namespace Neurons.UI {
    public class MUIDecayNeuron : MUIBoardNeuron {
        [Header("Dots"), SerializeField] private List<SpriteRenderer> dots;
        [SerializeField] private AnimationCurve dotSpawnEasing;
        [SerializeField] private float dotSpawnDuration;

        private int _turnCounter;
        private Sequence _hoverAnimation;

        private SDecayingNeuronData DecayData => RuntimeData.DataProvider as SDecayingNeuronData;
        
        #region Pooling

        public override void ToHoverLayer() {
            base.ToHoverLayer();
            dots.ForEach(d => {
                d.sortingLayerName = hoverSortingLayer;
                d.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }

        public override void ToBoardLayer() {
            base.ToBoardLayer();
            dots.ForEach(d => {
                d.sortingLayerName = aboveConnSortingLayer;
                d.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }
        
        public override void Default() {
            base.Default();
            ToBoardLayer();
            dots.ForEach(s => { s.gameObject.SetActive(true); s.transform.localScale = Vector3.one; });
            _turnCounter = 0;
            _hoverAnimation = null;
        }

        #endregion

        #region Animation

        public override async Task PlayAddAnimation() {
            base.PlayAddAnimation();
            dots.ForEach(d => d.transform.localScale = Vector3.zero);
            foreach (var dot in dots) {
                await dot.transform.DOScale(Vector3.one, dotSpawnDuration).SetEase(dotSpawnEasing).AsyncWaitForCompletion();
            }
        }

        public override async Task PlayTurnAnimation() {
            if (_turnCounter >= dots.Count) {
                return;
            }

            await dots[_turnCounter].transform.DOScale(0, dotSpawnDuration).AsyncWaitForCompletion();
            dots[_turnCounter].gameObject.SetActive(false);
            _turnCounter++;
        }

        public override Task PlayHoverAnimation() {
            if (_hoverAnimation != null && _hoverAnimation.IsPlaying()) {
                StopHoverAnimation();
            }
            _hoverAnimation = DOTween.Sequence();
            foreach (var dot in dots) {
                _hoverAnimation.Append(dot.transform.DOScale(0, dotSpawnDuration * 2));
            }

            foreach (var dot in dots) {
                _hoverAnimation.Append(dot.transform.DOScale(1, dotSpawnDuration * 2));
            }

            _hoverAnimation.SetAutoKill(false);
            _hoverAnimation.OnComplete(() => _hoverAnimation.Restart());
            return Task.CompletedTask;
        }

        public override void StopHoverAnimation() {
            _hoverAnimation?.Complete();
            _hoverAnimation?.Kill();
        }

        #endregion

        #region Sound

        public override void PlayAddSound() {
            base.PlayAddSound();
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = addVolume;
            s.Source.PlayOneShot(DecayData.GetDecayAddSound());
            AudioSpawner.ReleaseWhenDone(s);
        }

        public override void PlayRemoveSound() {
            base.PlayRemoveSound();
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = addVolume;
            s.Source.PlayOneShot(DecayData.GetDecayRemoveSound());
            AudioSpawner.ReleaseWhenDone(s);
        }

        #endregion
    }
}