using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.Animation;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Runtime;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using Main.Traits;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Main.MyHexBoardSystem.BoardSystem {
    
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
            // storyEventManager.Register(StoryEvents.OnInitStory, UpdateCurrentSP);
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
            // boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, OnBoardEffect);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
            // storyEventManager.Unregister(StoryEvents.OnInitStory, UpdateCurrentSP);
            // boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, OnBoardEffect);
        }

        #endregion


        private void OnBoardEffect(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            var boardEffect = storyEventArgs.Story.DecisionEffects.BoardEffect;
            foreach (var trait in boardEffect.Keys) {
                var neuronsInTrait = _neuronsController.GetTraitCount(trait);
                var effectStrength = GetTileAmountBasedOnNeurons(neuronsInTrait);
#if UNITY_EDITOR
                Assert.IsTrue(neuronsInTrait > 0 && effectStrength > 0 || neuronsInTrait == 0 && effectStrength == 0);
#endif
                if (boardEffect[trait] < 0) {
                    RemoveTilesFromTrait(trait, effectStrength);
                } else if (boardEffect[trait] > 0) {
                    AddTilesToTrait(trait, effectStrength);
                }
            }
            boardEventManager.Raise(ExternalBoardEvents.OnBoardModified, EventArgs.Empty);
        }

        private async void RemoveTilesFromTrait(ETrait trait, int amount) {
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
            AnimationManager.Register(RemoveTile(randomHex), EAnimationQueue.Tiles);
            return !isLastHex;
        }

        private async void AddTilesToTrait(ETrait trait, int amount) {
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
            AnimationManager.Register(AddTile(randomHex), EAnimationQueue.Tiles);
        }

        private int GetTileAmountBasedOnNeurons(int neuronAmount) {
            var frac = (float) neuronAmount / _neuronsController.CountNeurons;
            // return Mathf.RoundToInt(Mathf.SmoothStep(0, maxEffectStrength, frac));
            return Mathf.Clamp(Mathf.RoundToInt(effectScale * Mathf.Log(neuronAmount + 1)), 0, maxEffectStrength);
        }

        private async Task RemoveTile(Hex hex) {
            await Task.Delay(30);
            await _neuronsController.RemoveNeuron(hex);
            _boardController.RemoveTile(hex);
        }

        private async Task AddTile(Hex hex) {
            await Task.Delay(30);
            _boardController.AddTile(hex);

        }
    }
}