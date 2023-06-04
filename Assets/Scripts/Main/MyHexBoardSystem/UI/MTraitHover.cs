using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
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

        [Header("Highlighting"), SerializeField]
        private TileBase hoverTile;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;
        
        private ITraitAccessor _traitAccessor;
        private Camera _cam;

        private ETrait? _currentHighlightedTrait;
        private IStoryPoint _currentSP;

        #region UnityMethods

        private void Awake() {
            _cam = Camera.main;
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnPointerEnter, OnShow);
            boardEventManager.Register(ExternalBoardEvents.OnPointerStay, OnUpdatePosition);
            boardEventManager.Register(ExternalBoardEvents.OnPointerExit, OnHide);
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, OnTileAdded);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerEnter, OnShow);
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerStay, OnUpdatePosition);
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerExit, OnHide);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddTile, OnTileAdded);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
        }

        #endregion

        #region EventHandlers

        private void OnShow(EventArgs eventData) {
            if (eventData is not OnBoardInputEventArgs inputEventArgs ||
                // when board is disabled don't show this effect
                ENeuronType.Undefined.Equals(currentNeuron.Type) ||
                _currentSP == null) {
                return;
            }
            
            // get mouse world pos
            var mouseWorldPos = _cam.ScreenToWorldPoint(inputEventArgs.EventData.position);
            // figure out which trait the hex is in
            var hoverTrait = _traitAccessor.WorldPosToTrait(mouseWorldPos);
            // only highlight deciding traits
            if (!hoverTrait.HasValue || !_currentSP.DecidingTraits.ContainsKey(hoverTrait.Value)) {
                return;
            }
            _currentHighlightedTrait = hoverTrait; 
            Show(_currentHighlightedTrait.Value);
        }

        private void OnHide(EventArgs args) {
            if (args is not OnBoardInputEventArgs inputEventArgs || !_currentHighlightedTrait.HasValue) {
                return;
            }
            Hide(_currentHighlightedTrait);
        }

        private void OnUpdatePosition(EventArgs args) {
            if (args is not OnBoardPointerStayEventArgs pointerStayEventArgs || _currentSP == null) {
                return;
            }

            var mouseWorldPos = _cam.ScreenToWorldPoint(pointerStayEventArgs.MousePosition);
            var newTrait = _traitAccessor.WorldPosToTrait(mouseWorldPos);
            if (!newTrait.HasValue || newTrait == _currentHighlightedTrait) {
                return;
            }
            RefreshEffect(newTrait.Value);
        }

        private void OnTileRemoved(EventArgs eventArgs) {
            if (eventArgs is not OnTileModifyEventArgs tileEventArgs) {
                return;
            }

            if (_currentHighlightedTrait.HasValue) {
                Hide(_currentHighlightedTrait);
            }
        }
        
        private void OnTileAdded(EventArgs eventArgs) {
            if (eventArgs is not OnTileModifyEventArgs tileEventArgs) {
                return;
            }

            if (_currentHighlightedTrait.HasValue) {
                RefreshEffect(_currentHighlightedTrait.Value);
            }
        }

        private void OnInitStory(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentSP = storyEventArgs.Story;
        }

        #endregion

        private void Show(ETrait? trait) {
            if (!trait.HasValue) {
                return;
            }
            _traitAccessor.SetTiles(trait.Value, hoverTile, BoardConstants.TraitHoverTileLayer);
        }

        private void Hide(ETrait? trait) {
            if (!trait.HasValue) {
                return;
            }
            _traitAccessor.SetTiles(trait.Value, null, BoardConstants.TraitHoverTileLayer);
            _currentHighlightedTrait = null;
        }

        private void RefreshEffect(ETrait newTrait) {
            Hide(_currentHighlightedTrait);
            if (!_currentSP.DecidingTraits.ContainsKey(newTrait)) {
                return;
            }
            _currentHighlightedTrait = newTrait;
            Show(_currentHighlightedTrait);
        }
    }
}