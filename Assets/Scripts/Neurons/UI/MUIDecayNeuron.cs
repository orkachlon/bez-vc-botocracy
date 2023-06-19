using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Types.Board;
using UnityEngine;

namespace Neurons.UI {
    public class MUIDecayNeuron : MUIBoardNeuron {
        [Header("Dots"), SerializeField] private List<SpriteRenderer> dots;
        [SerializeField] private AnimationCurve dotSpawnEasing;
        [SerializeField] private float dotSpawnDuration;

        private int _turnCounter;
        private Sequence _hoverAnimation;

        protected override void UpdateView() {
            dots.ForEach(d => { d.gameObject.SetActive(true); d.transform.localScale = Vector3.one; });
        }

        public override void SetRuntimeElementData(IBoardElement data) {
            base.SetRuntimeElementData(data);
            _turnCounter = 0;
            _hoverAnimation = null;
        }

        public override void ToHoverLayer() {
            base.ToHoverLayer();
            dots.ForEach(d => d.sortingOrder = hoverSortingOrder + 1);
        }

        public override void ToBoardLayer() {
            base.ToBoardLayer();
            dots.ForEach(d => d.sortingOrder = boardSortingOrder + 2);
        }

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
    }
}