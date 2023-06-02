using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.Neurons;
using Main.StoryPoints;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.MyHexBoardSystem.UI {
    
    [RequireComponent(typeof(MTraitAccessor))]
    public class MBoardHighlighter : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        // maybe change the tile entirely instead of just the color
        [Header("Visuals"), SerializeField] private Color nonDecidingTraitColor;
        [SerializeField] private Color currentDecidingTraitColor;
        [SerializeField] private Color goodTraitColor;
        [SerializeField] private Color badTraitColor;
        
        private ITraitAccessor _traitAccessor;
        private readonly Dictionary<ETrait, Color> _previousColors = new();
        private IStoryPoint _currentSP;
        private ETrait _currentMaxTrait;

        #region UnityMethods

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            storyEventManager.Register(StoryEvents.OnInitStory, ResetMaxTraitMarking);
            storyEventManager.Register(StoryEvents.OnEvaluate, RevertNonDecidingTraits);
            neuronEventManager.Register(NeuronEvents.OnNeuronPlaced, OnMarkMaxTraitBoardBroadcast);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            storyEventManager.Unregister(StoryEvents.OnInitStory, ResetMaxTraitMarking);
            storyEventManager.Unregister(StoryEvents.OnEvaluate, RevertNonDecidingTraits);
            neuronEventManager.Unregister(NeuronEvents.OnNeuronPlaced, OnMarkMaxTraitBoardBroadcast);
        }

        #endregion


        #region EventHandlers

        /// <summary>
        ///     Reverts colored tiles to their original color
        /// </summary>
        private void RevertNonDecidingTraits(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }
            var story = storyEventArgs.Story;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                if (story.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                RevertColor(trait);
            }
        }

        /// <summary>
        ///     Marks traits that can't decide the current event
        /// </summary>
        private void MarkNonDecidingTraits(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentSP = storyEventArgs.Story;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                if (_currentSP.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                CacheColors(trait);
                _traitAccessor.SetColor(trait, nonDecidingTraitColor);
            }
        }

        private void ResetMaxTraitMarking(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            RevertColor(_currentMaxTrait);
            MarkMaxDecidingTrait();
        }
        
        private void OnMarkMaxTraitBoardBroadcast(EventArgs args) {
            if (args is not NeuronEventArgs neuronEventArgs) {
                return;
            }
            MarkMaxDecidingTrait();
        }

        #endregion

        private void MarkMaxDecidingTrait() {
            var maxTraits = _traitAccessor.GetMaxNeuronsTrait(_currentSP.DecidingTraits.Keys).ToArray();
            if (!maxTraits.Contains(_currentMaxTrait)) {
                RevertColor(_currentMaxTrait);
                // save new maximum
                _currentMaxTrait = maxTraits[Random.Range(0, maxTraits.Length - 1)];
            }

            CacheColors(_currentMaxTrait);
            _traitAccessor.SetColor(_currentMaxTrait, currentDecidingTraitColor);
        }

        private void CacheColors(ETrait trait) {
            if (_previousColors.ContainsKey(trait)) { // do not overwrite colors
                return;
            }
            _previousColors[trait] = _traitAccessor.GetColor(trait);
        }

        private void RevertColor(ETrait trait) {
            if (!_previousColors.ContainsKey(trait)) {
                return;
            }
            _traitAccessor.SetColor(trait, _previousColors[trait]);
            _previousColors.Remove(trait);
        }
    }
}