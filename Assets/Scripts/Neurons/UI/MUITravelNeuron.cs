using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using UnityEngine;

namespace Neurons.UI {
    public class MUITravelNeuron : MUIBoardNeuron {
        [Header("Probes"), SerializeField] private List<SpriteRenderer> probes;
        [SerializeField] private AnimationCurve probeEasing;
        [SerializeField] private float probeDuration;
        [SerializeField] private List<SpriteRenderer> lines;
        
        private Sequence _hoverAnimation;
        private Coroutine _hoverCoroutine;
        private int _turnCounter;

        private STravelNeuronData TravelData => RuntimeData.DataProvider as STravelNeuronData;
        
        
        public override void ToHoverLayer() {
            base.ToHoverLayer();
            neuronFace.sortingOrder++;
            probes.ForEach(p => {
                p.sortingLayerName = hoverSortingLayer;
                p.sortingOrder = neuronFace.sortingOrder - 1;
            });
            lines.ForEach(l => {
                l.sortingLayerName = hoverSortingLayer;
                l.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder++;
            probes.ForEach(p => {
                p.sortingLayerName = belowConnSortingLayer;
                p.sortingOrder = SpriteRenderer.sortingOrder + 1;
            });
            lines.ForEach(l => {
                l.sortingLayerName = aboveConnSortingLayer;
                l.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }

        public override async Task PlayAddAnimation() {
            probes.ForEach(p => p.transform.localScale = Vector3.one);
            var probeAnimations = probes
                .Select(probe => DOTween.Sequence(this)
                    .Append(probe.transform
                        .DORotate(probe.transform.eulerAngles + Vector3.forward * 60, probeDuration * 0.2f)
                        .SetEase(probeEasing))
                    .AsyncWaitForCompletion())
                .ToList();
            await base.PlayAddAnimation();
            await Task.WhenAll(probeAnimations);
        }

        public override async Task PlayMoveAnimation(Vector3 fromPos, Vector3 toPos) {
            probes.ForEach(p => p.transform.DOScale(Vector3.zero, moveAnimationDuration).SetLoops(2, LoopType.Yoyo));
            transform.DOMove(toPos, 0.25f);
            await transform.DOScale(0.5f, moveAnimationDuration).SetLoops(2, LoopType.Yoyo).AsyncWaitForCompletion();
        }

        public override Task PlayHoverAnimation() {
            _hoverCoroutine = StartCoroutine(HoverAnimation());
            return Task.CompletedTask;
        }

        public override void StopHoverAnimation() {
            if (_hoverCoroutine != null) {
                StopCoroutine(_hoverCoroutine);
            }

            KillHoverAnimationSequence();
        }

        public override async Task PlayTurnAnimation() {
            if (_turnCounter >= lines.Count) {
                await Task.WhenAll(probes.Select(probe => probe.transform
                    .DOScale(Vector3.zero, probeDuration * 0.1f)
                    .SetEase(probeEasing)
                    .AsyncWaitForCompletion()));
                return;
            }

            _turnCounter++;
            lines[_turnCounter - 1].transform.DOScale(0, probeDuration * 0.2f).OnComplete(() => lines[_turnCounter - 1].gameObject.SetActive(false));
        }

        public override void Default() {
            base.Default();
            _turnCounter = 0;
            for (var i = 0; i < probes.Count; i++) {
                probes[i].gameObject.SetActive(true);
                probes[i].transform.localScale = Vector3.one;
                probes[i].transform.eulerAngles = Vector3.forward * (60 * i);
                lines[i].gameObject.SetActive(true);
                lines[i].transform.localScale = Vector3.one;
            }
        }

        public Task DepleteTurns() {
            foreach (var l in lines.Where(l => l.gameObject.activeInHierarchy)) {
                l.transform.DOScale(Vector3.zero, probeDuration);
            }

            probes.ForEach(p => p.transform.DOScale(Vector3.zero, probeDuration));
            return Task.CompletedTask;
        }

        private IEnumerator HoverAnimation() {
            while (true) {
                if (_hoverAnimation != null) {
                    KillHoverAnimationSequence();
                }
                
                _hoverAnimation = DOTween.Sequence(this);
                var randomAngle = 60 * Random.Range(1, 4) * (Random.value > 0.5f ? 1 : -1);
                foreach (var probe in probes) {
                    _hoverAnimation.Insert(0, probe.transform
                        .DORotate(Vector3.forward * randomAngle, probeDuration, RotateMode.LocalAxisAdd)
                        .SetEase(probeEasing));
                }

                _hoverAnimation.SetAutoKill(false);
                _hoverAnimation.AppendInterval(probeDuration);
                _hoverAnimation.Play();

                yield return new WaitWhile(_hoverAnimation.IsPlaying);
            }
        }

        private void KillHoverAnimationSequence() {
            _hoverAnimation?.Complete();
            _hoverAnimation?.Kill();
            _hoverAnimation = null;
        }
    }
}