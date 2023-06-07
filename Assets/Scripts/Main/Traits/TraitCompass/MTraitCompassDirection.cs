﻿using Core.EventSystem;
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

        private Color _baseColor, _currentSPColor;
        private Image _image;

        public bool HasEffect { get; set; }
        
        private void Awake() {
            _image = GetComponent<Image>();
            _image.alphaHitTestMinimumThreshold = 0.5f;
            _baseColor = _image.color;
        }
        
        private void OnEnable() {
            _currentSPColor = HasEffect ? _baseColor : Color.gray;
            _image.color = _currentSPColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            _image.color =  HasEffect ? highlightColor : _currentSPColor;
            if (!HasEffect) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
        }

        public void OnPointerExit(PointerEventData eventData) {
            _image.color = _currentSPColor;
            if (!HasEffect) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }
    }
}