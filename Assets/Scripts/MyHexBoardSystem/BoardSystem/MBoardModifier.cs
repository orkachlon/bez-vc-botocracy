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
            var isOutOfTiles = _boardController.GetTraitTileCount(trait) <= amount;
            var removeTasks = new List<Task>();
            for(var i = 0; i < amount; i++) {
                removeTasks.Add(RemoveTileFromTrait(trait, i * 100));
            }

            await Task.WhenAll(removeTasks);
            if (isOutOfTiles) {
                // lose game
                boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
            }
        }
        
        private async Task RemoveTileFromTrait(ETrait trait, int delay = 0) {
            var edgeHexes = _boardController.Manipulator
                .GetEdge(ITraitAccessor.TraitToDirection(trait));
            if (edgeHexes.Length == 0) {
                return;
            }
            var randomHex = edgeHexes[Random.Range(0, edgeHexes.Length)];
            await AnimationManager.Register(RemoveTile(randomHex, delay));
        }

        private async Task AddTilesToTrait(ETrait trait, int amount) {
            var addTilesTasks = new List<Task>();
            for (var i = 0; i < amount; i++) {
                addTilesTasks.Add(AddTileToTrait(trait, i * 100));
            }

            await Task.WhenAll(addTilesTasks);
        }
        
        private async Task AddTileToTrait(ETrait trait, int delay = 0) {
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
            await AnimationManager.Register(AddTile(randomHex, delay));
        }

        private int GetTileAmountBasedOnNeurons(int neuronAmount) {
            // var frac = (float) neuronAmount / _neuronsController.CountNeurons;
            // return Mathf.RoundToInt(Mathf.SmoothStep(0, maxEffectStrength, frac));
            return Mathf.Clamp(Mathf.RoundToInt(effectScale * Mathf.Log(neuronAmount + 1)), 0, maxEffectStrength);
        }

        private async Task RemoveTile(Hex hex, int delay = 0) {
            await _neuronsController.RemoveNeuron(hex);
            await Task.Delay(delay);
            _boardController.RemoveTile(hex);
        }

        private async Task AddTile(Hex hex, int delay = 0) {
            await Task.Delay(delay);
            _boardController.AddTile(hex);

        }
    }
}