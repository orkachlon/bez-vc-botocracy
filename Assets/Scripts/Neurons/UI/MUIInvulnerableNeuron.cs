using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Neurons.UI {
    public class MUIInvulnerableNeuron : MUIBoardNeuron {
        [Header("Rings"), SerializeField] private List<SpriteRenderer> rings;
        [SerializeField] private AnimationCurve ringEasing;
        [SerializeField] private float ringDuration;
        
        private List<Tween> _constantAnimations;
        
        private SInvulnerableNeuronData InvulData => RuntimeData.DataProvider as SInvulnerableNeuronData;

        protected override void OnDestroy() {
            base.OnDestroy();
            _constantAnimations?.ForEach(a => {
                a?.Kill();
            });
            _constantAnimations?.Clear();
        }

        public override void ToHoverLayer() {
            base.ToHoverLayer();
            rings.ForEach(s => { 
                s.sortingLayerName = hoverSortingLayer;
                s.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            rings.ForEach(s => {
                s.sortingLayerName = aboveConnSortingLayer;
                s.sortingOrder = neuronFace.sortingOrder + 1;
            });
        }
        
        public override async Task PlayAddAnimation() {
            await base.PlayAddAnimation();
            _constantAnimations = new List<Tween>();
#if UNITY_EDITOR
            Assert.IsTrue(rings.Count > 1);
#endif
            var interval = 1f / (rings.Count - 1);
            foreach (var ring in rings) {
                await Task.Delay(Mathf.RoundToInt(1000 * (ringDuration * interval)));
                _constantAnimations.Add(GetRingAnimation(ring.transform));
            }
        }

        private Tween GetRingAnimation(Transform ring) {
            return ring.DOScale(Vector3.zero, ringDuration).SetEase(ringEasing).SetLoops(-1);
        }
        
        public override void Default() {
            base.Default();
            ToBoardLayer();
            rings.ForEach(s => { s.gameObject.SetActive(true); s.transform.localScale = Vector3.one; });
            _constantAnimations = null;
        }
    }
}