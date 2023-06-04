using System;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
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
        private System.Random _rndGenerator;

        #region UnityMethods

        private void Awake() {
            _boardController = GetComponent<INeuronBoardController>();
            _neuronsController = GetComponent<IBoardNeuronsController>();
            _rndGenerator = new System.Random();
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

            var boardEffect = storyEventArgs.Story.DecisionEffects.BoardEffect;
            foreach (var trait in boardEffect.Keys) {
                var effectStrength = GetTileAmountBasedOnNeurons(_neuronsController.GetTraitCount(trait));
                if (boardEffect[trait] < 0) {
                    RemoveTilesFromTrait(trait, effectStrength);
                } else if (boardEffect[trait] > 0) {
                    AddTilesToTrait(trait, effectStrength);
                }
            }
        }

        private void RemoveTilesFromTrait(ETrait trait, int amount) {
            for(var i = 0; i < amount; i++) {
                if (!RemoveTraitTile(trait)) {
                    // lose game
                    boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
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

        private void AddTilesToTrait(ETrait trait, int amount) {
            for (var i = 0; i < amount; i++) {
                AddTileToTrait(trait);
            }
        }
        
        private void AddTileToTrait(ETrait trait) {
            var edgeHexes = _boardController.Manipulator
                .GetEdge(ITraitAccessor.TraitToDirection(trait));
            var surroundingHexes = _boardController.Manipulator.GetSurroundingHexes(edgeHexes, true);
            var onlyEmptySurroundingHexes = surroundingHexes.Where(h => !_boardController.Board.HasPosition(h));
            var onlyContainedInTrait = onlyEmptySurroundingHexes.Where(h =>
                    ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(h)) == trait)
                .ToArray();
            var randomHex = onlyContainedInTrait[Random.Range(0, onlyContainedInTrait.Length)];
            _boardController.AddTile(randomHex);
        }

        private int GetTileAmountBasedOnNeurons(int neuronAmount) {
            return Mathf.RoundToInt(5 * Mathf.Log(neuronAmount + 1));
        }
    }
}