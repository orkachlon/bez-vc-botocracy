﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using Main.StoryPoints;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.MyHexBoardSystem.UI {
    
    [RequireComponent(typeof(MNeuronBoardController)), RequireComponent(typeof(MBoardNeuronsController))]
    public class MBoardHighlighter : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        // maybe change the tile entirely instead of just the color
        [Header("Visuals"), SerializeField] private Color nonDecidingTraitColor;
        [SerializeField] private Color currentDecidingTraitColor;
        [SerializeField] private Color goodTraitColor;
        [SerializeField] private Color badTraitColor;
        
        private INeuronBoardController _boardController;
        private IBoardNeuronController _neuronController;
        private readonly Dictionary<Hex, Color> _previousColors = new();
        private IStoryPoint _currentSP;
        private ETraitType _currentMaxTrait;

        #region UnityMethods

        private void Awake() {
            _boardController = GetComponent<INeuronBoardController>();
            _neuronController = GetComponent<IBoardNeuronController>();
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
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                if (story.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                var traitTiles = _boardController.Manipulator.GetTriangle(INeuronBoardController.TraitToDirection(trait));
                RevertColor(traitTiles);
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
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                if (_currentSP.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                var traitHexes = _boardController.Manipulator.GetTriangle(INeuronBoardController.TraitToDirection(trait));
                CacheColors(traitHexes);
                _boardController.SetColor(traitHexes, nonDecidingTraitColor);
            }
        }

        private void ResetMaxTraitMarking(EventArgs args) {
            if (args is not StoryEventArgs storyEventArgs) {
                return;
            }

            RevertColor(_boardController.Manipulator.GetTriangle(INeuronBoardController.TraitToDirection(_currentMaxTrait)));
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
            var maxTraits = _neuronController.GetMaxTrait(_currentSP.DecidingTraits.Keys).ToArray();
            if (!maxTraits.Contains(_currentMaxTrait)) {
                RevertColor(
                    _boardController.Manipulator.GetTriangle(
                        INeuronBoardController.TraitToDirection(_currentMaxTrait)));
                // save new maximum
                _currentMaxTrait = maxTraits[Random.Range(0, maxTraits.Length - 1)];
            }

            var maxTraitHexes = _boardController.Manipulator
                .GetTriangle(INeuronBoardController.TraitToDirection(_currentMaxTrait));

            CacheColors(maxTraitHexes);
            _boardController.SetColor(maxTraitHexes, currentDecidingTraitColor);
        }

        private void CacheColors(Hex[] hexes) {
            foreach (var hex in hexes) {
                if (_previousColors.ContainsKey(hex)) { // do not overwrite colors
                    continue;
                }
                _previousColors[hex] = _boardController.GetColor(hex);
            }
        }

        private void RevertColor(Hex[] hexes) {
            foreach (var hex in hexes) {
                if (!_previousColors.ContainsKey(hex)) {
                    continue;
                }
                _boardController.SetColor(hex, _previousColors[hex]);
                _previousColors.Remove(hex);
            }
        }
    }
}