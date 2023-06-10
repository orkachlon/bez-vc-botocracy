using System;
using System.Collections.Generic;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.UI.TraitHover {
    
    /// <summary>
    ///     This class highlights the traits affected by the next neuron placement
    /// </summary>
    [RequireComponent(typeof(ITraitAccessor))]
    public class MTraitHover : MonoBehaviour {
        
        [Header("Current Neuron"), SerializeField]
        private SNeuronData currentNeuron;

        [Header("Highlighting"), SerializeField] private TileBase positiveTile;
        [SerializeField] private TileBase negativeTile;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        [SerializeField] private SEventManager storyEventManager;

        private ITraitAccessor TraitAccessor { get; set; }
        private Camera _cam;

        private ETrait? _currentHighlightedTrait;
        private readonly HashSet<ETrait> _currentPositive = new ();
        private readonly HashSet<ETrait> _currentNegative = new ();
        private IStoryPoint _currentSP;

        #region UnityMethods

        private void Awake() {
            _cam = Camera.main;
            TraitAccessor = GetComponent<ITraitAccessor>();
            // get the references to the effects
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassEnter, OnShow);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassExit, OnHide);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassHide, OnHide);
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassEnter, OnShow);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassExit, OnHide);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassHide, OnHide);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
        }

        #endregion

        #region EventHandlers

        private void OnShow(EventArgs eventData) {
            if (eventData is not TraitCompassHoverEventArgs traitHoverArgs ||
                // when board is disabled don't show this effect
                ENeuronType.Undefined.Equals(currentNeuron.Type) ||
                _currentSP == null) {
                return;
            }

            var hoverTrait = traitHoverArgs.HighlightedTrait;
            // only highlight deciding traits
            if (!hoverTrait.HasValue || !_currentSP.DecidingTraits.ContainsKey(hoverTrait.Value)) {
                return;
            }
            CacheHoverData(hoverTrait.Value);
            Show();
        }

        private void OnHide(EventArgs _) {
            if (!_currentHighlightedTrait.HasValue) {
                return;
            }
            Hide();
            ClearHoverCache();
        }

        private void CacheHoverData(ETrait hoverTrait) {
            _currentHighlightedTrait = hoverTrait;
            var affectedTraits = _currentSP.DecidingTraits[hoverTrait].BoardEffect;
            foreach (var trait in affectedTraits.Keys) {
                if (affectedTraits[trait] > 0) {
                    _currentPositive.Add(trait);
                }
                else if (affectedTraits[trait] < 0) {
                    _currentNegative.Add(trait);
                }
            }
        }

        private void ClearHoverCache() {
            _currentHighlightedTrait = null;
            _currentPositive.Clear();
            _currentNegative.Clear();
        }
        
        private void OnInitStory(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentSP = storyEventArgs.Story;
        }

        #endregion

        public void Show() {
            if (!_currentHighlightedTrait.HasValue) {
                return;
            }
            // TraitAccessor.SetTiles(_currentHighlightedTrait.Value, hoverTile, BoardConstants.TraitHoverTileLayer);
            foreach (var t in _currentPositive) {
                TraitAccessor.SetTraitTiles(t, positiveTile, BoardConstants.SPEffectHoverTileLayer);
            }
            foreach (var t in _currentNegative) {
                TraitAccessor.SetTraitTiles(t, negativeTile, BoardConstants.SPEffectHoverTileLayer);
            }
        }

        public void Hide() {
            if (!_currentHighlightedTrait.HasValue) {
                return;
            }
            // TraitAccessor.SetTiles(_currentHighlightedTrait.Value, null, BoardConstants.TraitHoverTileLayer);
            foreach (var t in _currentPositive) {
                TraitAccessor.SetTraitTiles(t, null, BoardConstants.SPEffectHoverTileLayer);
            }
            foreach (var t in _currentNegative) {
                TraitAccessor.SetTraitTiles(t, null, BoardConstants.SPEffectHoverTileLayer);
            }
        }
    }
}