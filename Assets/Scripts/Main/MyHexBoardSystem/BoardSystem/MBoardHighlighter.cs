using System;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.StoryPoints;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardSystem {
    
    [RequireComponent(typeof(MNeuronBoardController))]
    public class MBoardHighlighter : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        // maybe change the tile entirely instead of just the color
        [Header("Visuals"), SerializeField] private Color nonDecidingTraitColor;
        
        private INeuronBoardController _boardController;
        private readonly Dictionary<Hex, Color> _previousColors = new();

        #region UnityMethods

        private void Awake() {
            _boardController = GetComponent<INeuronBoardController>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            storyEventManager.Register(StoryEvents.OnEvaluate, RevertTileColors);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, MarkNonDecidingTraits);
            storyEventManager.Unregister(StoryEvents.OnEvaluate, RevertTileColors);
        }

        #endregion


        #region EventHandlers

        private void RevertTileColors(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }
            var story = storyEventArgs.Story;
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                if (story.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                var traitTiles = _boardController.BoardManipulation.GetTriangle(INeuronBoardController.TraitToDirection(trait));
                foreach (var hex in traitTiles) {
                    _boardController.SetColor(hex, _previousColors[hex]);
                    _previousColors.Remove(hex);
                }
            }
        }

        private void MarkNonDecidingTraits(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }

            var story = storyEventArgs.Story;
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                if (story.DecidingTraits.ContainsKey(trait)) {
                    continue;
                }

                var traitTiles = _boardController.BoardManipulation.GetTriangle(INeuronBoardController.TraitToDirection(trait));
                foreach (var hex in traitTiles) {
                    _previousColors[hex] = _boardController.GetColor(hex);
                }
                _boardController.SetColor(traitTiles, nonDecidingTraitColor);
            }
        }

        #endregion
    }
}