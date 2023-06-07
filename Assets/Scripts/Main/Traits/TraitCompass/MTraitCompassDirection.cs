using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Traits.TraitCompass {
    public class MTraitCompassDirection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField] private ETrait trait;
        [SerializeField] private Color highlightColor;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private Color _baseColor;

        private Image _image;

        private void Awake() {
            _image = GetComponent<Image>();
            _image.alphaHitTestMinimumThreshold = 0.5f;
            _baseColor = _image.color;
        }

        private void OnEnable() {
            _image.color = _baseColor;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _image.color = highlightColor;
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
        }

        public void OnPointerExit(PointerEventData eventData) {
            _image.color = _baseColor;
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }
    }
}