using System;
using System.Linq;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.StoryPoints;
using Main.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.MyHexBoardSystem.BoardSystem {
    
    [RequireComponent(typeof(INeuronBoardController)), RequireComponent(typeof(IBoardNeuronsController))]
    public class MBoardModifier : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        private INeuronBoardController _boardController;
        private IBoardNeuronsController _neuronsController;

        #region UnityMethods

        private void Awake() {
            _boardController = GetComponent<INeuronBoardController>();
            _neuronsController = GetComponent<IBoardNeuronsController>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        #endregion


        private void OnBoardEffect(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            foreach (var trait in storyEventArgs.Story.DecisionEffects.BoardEffect.Keys) {
                if (storyEventArgs.Story.DecisionEffects.BoardEffect[trait] < 0) {
                    if (!RemoveTraitTile(trait)) {
                        // lose game
                        boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
                    }
                } else if (storyEventArgs.Story.DecisionEffects.BoardEffect[trait] > 0) {
                    // Add tile
                    AddTraitTile(trait);
                }
            }
        }
        
        /// <summary>
        ///     Returns false if trait has no more tiles
        /// </summary>
        private bool RemoveTraitTile(ETrait trait) {
            var edgeHexes = _boardController.Manipulator
                .GetEdge(ITraitAccessor.TraitToDirection(trait));
            if (edgeHexes.Length == 0) {
                return false;
            }
            var isLastHex = edgeHexes.Length == 1;
            
            var randomHex = edgeHexes[Random.Range(0, edgeHexes.Length)];
            _neuronsController.RemoveElement(randomHex);
            _boardController.RemoveTile(randomHex);
            return !isLastHex;
        }

        private void AddTraitTile(ETrait trait) {
            var edgeHexes = _boardController.Manipulator
                .GetEdge(ITraitAccessor.TraitToDirection(trait));
            var surroundingHexes = _boardController.Manipulator.GetSurroundingHexes(edgeHexes, true);
            var onlyEmptySurroundingHexes = surroundingHexes.Where(h => !_boardController.Board.HasPosition(h)).ToArray();
            var randomHex = onlyEmptySurroundingHexes[Random.Range(0, onlyEmptySurroundingHexes.Length)];
            _boardController.AddTile(randomHex);
        }
    }
}