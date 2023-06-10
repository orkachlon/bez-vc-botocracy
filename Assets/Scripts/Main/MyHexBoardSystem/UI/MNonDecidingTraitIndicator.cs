using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.Neurons;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Main.MyHexBoardSystem.UI {
    
    [RequireComponent(typeof(MTraitAccessor))]
    public class MNonDecidingTraitIndicator : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        // maybe change the tile entirely instead of just the color
        [Header("Visuals"), SerializeField] private TileBase nonDecidingTraitTile;
        [SerializeField] private Color nonDecidingTraitColor;
        
        private ITraitAccessor _traitAccessor;
        private readonly Dictionary<ETrait, Color> _previousColors = new();
        private readonly Dictionary<ETrait, TileBase> _previousTiles = new();
        private IStoryPoint _currentSP;
        private ETrait _currentMaxTrait;

        #region UnityMethods

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            // storyEventManager.Register(StoryEvents.OnInitStory, ResetMaxTraitMarking);
            storyEventManager.Register(StoryEvents.OnEvaluate, RevertNonDecidingTraits);
            // neuronEventManager.Register(NeuronEvents.OnNeuronPlaced, OnMarkMaxTraitBoardBroadcast);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            // storyEventManager.Unregister(StoryEvents.OnInitStory, ResetMaxTraitMarking);
            storyEventManager.Unregister(StoryEvents.OnEvaluate, RevertNonDecidingTraits);
            // neuronEventManager.Unregister(NeuronEvents.OnNeuronPlaced, OnMarkMaxTraitBoardBroadcast);
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

                RevertTiles(trait);
                // RevertColor(trait);
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

                CacheTiles(trait);
                _traitAccessor.SetTraitTiles(trait, nonDecidingTraitTile);
                // TraitAccessor.SetColor(trait, nonDecidingTraitColor);
            }
        }

        #endregion

        private void CacheTiles(ETrait trait) {
            if (_previousTiles.ContainsKey(trait)) { // do not overwrite colors
                return;
            }
            _previousTiles[trait] = _traitAccessor.GetTraitTile(trait);
        }

        private void RevertColor(ETrait trait) {
            if (!_previousColors.ContainsKey(trait)) {
                return;
            }
            _traitAccessor.SetTraitColor(trait, Color.white);
            _previousColors.Remove(trait);
        }

        private void RevertTiles(ETrait trait) {
            if (!_previousTiles.ContainsKey(trait)) {
                return;
            }
            _traitAccessor.SetTraitTiles(trait, _previousTiles[trait]);
            _previousTiles.Remove(trait);
        }
    }
}