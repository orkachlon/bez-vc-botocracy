using System.Collections.Generic;
using System.Linq;
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
        
        public override void ToHoverLayer() {
            base.ToHoverLayer();
            neuronFace.sortingOrder = hoverSortingOrder + 2;
            rings.ForEach(s => s.sortingOrder = hoverSortingOrder + 3);
        }
        
        public override void ToBoardLayer() {
            base.ToBoardLayer();
            neuronFace.sortingOrder = boardSortingOrder + 3;
            rings.ForEach(s => s.sortingOrder = boardSortingOrder + 4);
        }
        
        public override async Task PlayAddAnimation() {
            await base.PlayAddAnimation();
            _constantAnimations = new List<Tween>();
            Assert.IsTrue(rings.Count > 1);
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