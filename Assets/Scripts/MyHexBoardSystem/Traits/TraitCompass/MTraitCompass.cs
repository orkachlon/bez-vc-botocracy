using System;
using System.Collections.Generic;
using System.Threading;
using Core.EventSystem;
using Core.Utils.DataStructures;
using DG.Tweening;
using MyHexBoardSystem.Events;
using Types.Trait;
using UnityEngine;
using UnityEngine.Assertions;

namespace MyHexBoardSystem.Traits.TraitCompass {
    public class MTraitCompass : MonoBehaviour {
        [SerializeField] private CompassDirectionDict compassDirections;
        [SerializeField] private float mouseDistanceTillDisabled;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        public RectTransform RectTransform { get; private set; }

        private Sequence _showSeq;
        
        private void OnEnable() {
            RectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(RectTransform);
        }

        private void OnDisable() {
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassHide, new TraitCompassHoverEventArgs(null));
        }

        private void Update() {
            if (Vector2.Distance(UnityEngine.Input.mousePosition, RectTransform.position) > mouseDistanceTillDisabled) {
                Hide();
            }
        }
        
        public void Show(Vector3 position) {
            gameObject.SetActive(true);
            _showSeq?.Complete();
            _showSeq?.Kill();
            RectTransform.localScale = Vector3.zero;
            RectTransform.position = position;
            _showSeq = DOTween.Sequence(RectTransform.DOScale(1, 0.1f));
        }

        public void Hide() {
            _showSeq?.Complete();
            _showSeq?.Kill();
            _showSeq = DOTween.Sequence(RectTransform.DOScale(0, 0.1f).OnComplete(() => gameObject.SetActive(false)));
        }

        public void SetDecidingTraits(IEnumerable<ETrait> decidingTraits) {
            // reset all traits to false
            foreach (var trait in compassDirections.Keys) {
                compassDirections[trait].HasEffect = false;
            }
            
            // mark deciding traits
            foreach (var trait in decidingTraits) {
                if (!compassDirections.ContainsKey(trait)) {
                    continue;
                }
                compassDirections[trait].HasEffect = true;
            }
        }

        public void SetCurrentDecidingTraits(IEnumerable<ETrait> traits) {
            
        }
    }
    
    [Serializable]
    internal class CompassDirectionDict : UDictionary<ETrait, MTraitCompassDirection> { }
}