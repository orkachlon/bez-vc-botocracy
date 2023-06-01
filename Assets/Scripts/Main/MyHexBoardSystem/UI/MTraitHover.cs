using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.UI {
    
    /// <summary>
    ///     This class highlights the traits affected by the next neuron placement
    /// </summary>
    [RequireComponent(typeof(MTraitAccessor))]
    public class MTraitHover : MonoBehaviour {
        
        [Header("Current Neuron"), SerializeField]
        private SNeuronData currentNeuron;

        [Header("Tilemap and Tiles"), SerializeField]
        private TileBase hoverTile;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        
        private ITraitAccessor _traitAccessor;
        private Camera _cam;

        private ETraitType _currentHighlightedTrait;

        private void Awake() {
            _cam = Camera.main;
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnPointerEnter, OnShow);
            boardEventManager.Register(ExternalBoardEvents.OnPointerStay, OnUpdatePosition);
            boardEventManager.Register(ExternalBoardEvents.OnPointerExit, OnHide);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerEnter, OnShow);
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerStay, OnUpdatePosition);
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerExit, OnHide);
        }

        // todo consider checking if we are already highlighting this trait
        private void OnShow(EventArgs eventData) {
            if (eventData is not OnBoardInputEventArgs inputEventArgs) {
                return;
            }
            // when board is disabled don't show this effect
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
                return;
            }
            // get mouse world pos
            var mouseWorldPos = _cam.ScreenToWorldPoint(inputEventArgs.EventData.position);
            // figure out which trait the hex is in
            var hoverTrait = _traitAccessor.WorldPosToTrait(mouseWorldPos);
            if (!hoverTrait.HasValue) {
                return;
            }
            _currentHighlightedTrait = hoverTrait.Value; 
            Show(_currentHighlightedTrait);

        }

        private void OnHide(EventArgs args) {
            if (args is not OnBoardInputEventArgs inputEventArgs) {
                return;
            }
            Hide(_currentHighlightedTrait);
        }

        private void OnUpdatePosition(EventArgs args) {
            if (args is not OnBoardPointerStayEventArgs pointerStayEventArgs) {
                return;
            }

            var mouseWorldPos = _cam.ScreenToWorldPoint(pointerStayEventArgs.MousePosition);
            var newTrait = _traitAccessor.WorldPosToTrait(mouseWorldPos);
            if (!newTrait.HasValue || newTrait == _currentHighlightedTrait) {
                return;
            }
            Hide(_currentHighlightedTrait);
            _currentHighlightedTrait = newTrait.Value;
            Show(_currentHighlightedTrait);
        }

        private void Show(ETraitType trait) {
            _traitAccessor.SetTiles(trait, hoverTile, BoardConstants.TraitHoverTileLayer);
        }

        private void Hide(ETraitType trait) {
            _traitAccessor.SetTiles(trait, null, BoardConstants.TraitHoverTileLayer);
        }
    }
}