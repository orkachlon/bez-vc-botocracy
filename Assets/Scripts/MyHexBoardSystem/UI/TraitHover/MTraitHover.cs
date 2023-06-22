using System;
using System.Collections.Generic;
using Core.EventSystem;
using Events.Board;
using Events.Neuron;
using Events.SP;
using MyHexBoardSystem.BoardSystem.Interfaces;
using Types.Neuron.Runtime;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.UI.TraitHover {
    
    /// <summary>
    ///     This class highlights the traits affected by the next neuron placement
    /// </summary>
    [RequireComponent(typeof(ITraitAccessor))]
    public class MTraitHover : MonoBehaviour {
        [Header("Highlighting"), SerializeField] private TileBase positiveTile;
        [SerializeField] private TileBase negativeTile;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        private ITraitAccessor TraitAccessor { get; set; }
        private Camera _cam;

        private ETrait? _currentHighlightedTrait;
        private readonly HashSet<ETrait> _currentPositive = new ();
        private readonly HashSet<ETrait> _currentNegative = new ();
        private IBoardNeuron _currentNeuron;
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
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassEnter, OnShow);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassExit, OnHide);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassHide, OnHide);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        #endregion

        #region EventHandlers

        private void OnShow(EventArgs eventData) {
            if (eventData is not TraitCompassHoverEventArgs traitHoverArgs ||
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

        private void OnInitStory(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentSP = storyEventArgs.Story;
        }
        
        private void UpdateNextNeuron(EventArgs obj) {
            if (obj is not NeuronQueueEventArgs queueEventArgs) {
                return;
            }

            _currentNeuron = queueEventArgs.NeuronQueue.NextBoardNeuron;
        }

        #endregion

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