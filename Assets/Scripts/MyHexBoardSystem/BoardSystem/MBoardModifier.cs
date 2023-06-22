using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem.Interfaces;
using Types.Board;
using Types.Hex.Coordinates;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace MyHexBoardSystem.BoardSystem {
    
    [RequireComponent(typeof(INeuronBoardController)), RequireComponent(typeof(IBoardNeuronsController))]
    public class MBoardModifier : MonoBehaviour {

        [Header("Modification Controls"), SerializeField]
        private int maxEffectStrength;
        [SerializeField, Range(1, 5)] private float effectScale;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        private INeuronBoardController _boardController;
        private IBoardNeuronsController _neuronsController;
        private IStoryPoint _currentSP;

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

            var boardEffect = storyEventArgs.Story.DecisionEffects.BoardEffect;
            ModifyBoard(boardEffect);
        }

        private async void ModifyBoard(Dictionary<ETrait, int> boardEffect) {
            var tileEffectTasks = new List<Task>();
            foreach (var trait in boardEffect.Keys) {
                var neuronsInTrait = _neuronsController.GetTraitCount(trait);
                var effectStrength = GetTileAmountBasedOnNeurons(neuronsInTrait);
#if UNITY_EDITOR
                Assert.IsTrue(neuronsInTrait > 0 && effectStrength > 0 || neuronsInTrait == 0 && effectStrength == 0);
#endif
                if (effectStrength > _boardController.GetTraitTileCount(trait)) {
                    boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
                }
                if (boardEffect[trait] < 0) {
                    tileEffectTasks.Add(RemoveTilesFromTrait(trait, effectStrength));
                }
                else if (boardEffect[trait] > 0) {
                    tileEffectTasks.Add(AddTilesToTrait(trait, effectStrength));
                }
            }

            await Task.WhenAll(tileEffectTasks);
            boardEventManager.Raise(ExternalBoardEvents.OnBoardModified, EventArgs.Empty);
        }

        private async Task RemoveTilesFromTrait(ETrait trait, int amount) {
            for(var i = 0; i < amount; i++) {
                if (!RemoveTraitTile(trait)) {
                    // lose game
                    boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
                }

                await Task.Delay(100);
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
            AnimationManager.Register(RemoveTile(randomHex));
            return !isLastHex;
        }

        private async Task AddTilesToTrait(ETrait trait, int amount) {
            for (var i = 0; i < amount; i++) {
                AddTileToTrait(trait);
                await Task.Delay(100);
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
            
            // give priority to tiles with neighbors in order to promote island connection
            // var orderedByExistingNeighbors = onlyContainedInTrait
            //     .OrderByDescending(h =>
            //         BoardManipulationOddR<IBoardNeuron>.GetNeighboursStatic(h)
            //             .Count(n => _boardController.Board.HasPosition(n)))
            //     .ToArray();
            // var bestConnection = orderedByExistingNeighbors[0];
            AnimationManager.Register(AddTile(randomHex));
        }

        private int GetTileAmountBasedOnNeurons(int neuronAmount) {
            // var frac = (float) neuronAmount / _neuronsController.CountNeurons;
            // return Mathf.RoundToInt(Mathf.SmoothStep(0, maxEffectStrength, frac));
            return Mathf.Clamp(Mathf.RoundToInt(effectScale * Mathf.Log(neuronAmount + 1)), 0, maxEffectStrength);
        }

        private async Task RemoveTile(Hex hex) {
            var element = _neuronsController.Board.GetPosition(hex).Data;
            await _neuronsController.RemoveNeuron(hex);
            if (element != null) {
                await AnimationManager.WaitForElement(element);
            }
            await Task.Delay(30);
            _boardController.RemoveTile(hex);
        }

        private async Task AddTile(Hex hex) {
            await Task.Delay(30);
            _boardController.AddTile(hex);

        }
    }
}