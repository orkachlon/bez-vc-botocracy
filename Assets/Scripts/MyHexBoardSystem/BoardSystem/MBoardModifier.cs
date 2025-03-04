﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.SP;
using ExternBoardSystem.BoardSystem.Board;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
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
        [SerializeField, Range(1, 3)] private float addToRemoveRatio;

        [Header("Event Managers"), SerializeField]
        protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager boardEventManager;

        protected INeuronBoardController BoardController;
        protected IBoardNeuronsController NeuronsController;
        protected virtual ITraitAccessor TraitAccessor { get; private set; }
        
        #region UnityMethods

        protected virtual void Awake() {
            BoardController = GetComponent<INeuronBoardController>();
            NeuronsController = GetComponent<IBoardNeuronsController>();
            TraitAccessor = GetComponent<ITraitAccessor>();
        }

        protected virtual void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        protected virtual void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        #endregion

        #region EventHandlers

        private async void OnBoardEffect(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            if (IsGameLosingSP(storyEventArgs.Story)) {
                await RemoveAllTiles();
                return;
            }
            var boardEffect = storyEventArgs.Story.DecisionEffects.BoardEffect;
            ModifyBoard(boardEffect);
        }

        #endregion

        protected virtual async void ModifyBoard(Dictionary<ETrait, int> boardEffect) {
            var tileEffectTasks = new List<Task>();
            foreach (var trait in boardEffect.Keys) {
                var neuronsInTrait = NeuronsController.GetTraitCount(trait);
                var effectStrength = Mathf.Clamp(GetEffectStrengthBasedOnNeurons(neuronsInTrait), 0, BoardController.GetTraitTileCount(trait));
#if UNITY_EDITOR
                Assert.IsTrue(neuronsInTrait > 0 && effectStrength > 0 || neuronsInTrait == 0 && effectStrength == 0);
#endif
                if (boardEffect[trait] < 0) {
                    tileEffectTasks.Add(RemoveTilesFromTrait(trait, effectStrength));
                }
                else if (boardEffect[trait] > 0) {
                    tileEffectTasks.Add(AddTilesToTrait(trait, Mathf.RoundToInt(effectStrength * addToRemoveRatio)));
                }
                if (effectStrength > BoardController.GetTraitTileCount(trait)) {
                    boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
                }
            }

            await Task.WhenAll(tileEffectTasks);
            boardEventManager.Raise(ExternalBoardEvents.OnBoardModified, EventArgs.Empty);
        }

        // ReSharper disable once InconsistentNaming

        private bool IsGameLosingSP(IStoryPoint sp) {
            return sp.DecidingTraits.Values.All(decisionEffects =>
                decisionEffects.BoardEffect.Values.All(effect => effect == -1));
        }

        #region RemoveTiles

        private async Task RemoveAllTiles() {
            var tileRemoveTasks = EnumUtil.GetValues<ETrait>()
                .Select(trait => RemoveTilesFromTrait(trait, BoardController.GetTraitTileCount(trait), false));

            await Task.WhenAll(tileRemoveTasks);
            // lose game
            boardEventManager.Raise(ExternalBoardEvents.OnAllTraitsOutOfTiles, new AllTraitsOutOfTilesEventArgs());
        }

        public async Task RemoveTilesFromTrait(ETrait trait, int amount, bool withLoss = true) {
            var isOutOfTiles = BoardController.GetTraitTileCount(trait) <= amount;
            //var removeTasks = new List<Task>();
            for (var i = 0; i < amount; i++) {
                await RemoveTileFromTrait(trait);
            }

            if (withLoss && isOutOfTiles) {
                // lose game
                boardEventManager.Raise(ExternalBoardEvents.OnTraitOutOfTiles, new TraitOutOfTilesEventArgs(trait));
            }
        }

        protected virtual async Task RemoveTileFromTrait(ETrait trait, int delay = 0) {
            var edgeHexes = BoardController.Manipulator
                .GetEdge(TraitAccessor.TraitToDirection(trait));
            if (edgeHexes.Length == 0) {
                return;
            }
            var randomHex = edgeHexes[Random.Range(0, edgeHexes.Length)];
            await AnimationManager.Register(RemoveTile(randomHex, delay));
        }

        protected async Task RemoveTile(Hex hex, int delay = 0) {
            await NeuronsController.RemoveNeuron(hex);
            await Task.Delay(delay);
            await BoardController.RemoveTile(hex);
        }

        #endregion

        #region AddTiles

        public virtual async Task AddTilesToTrait(ETrait trait, int amount) {
            var addTilesTasks = new List<Task>();
            for (var i = 0; i < amount; i++) {
                addTilesTasks.Add(AddTileToTrait(trait, RandomEmptyHexSelector, i * 100));
            }

            await Task.WhenAll(addTilesTasks);
        }

        public async Task AddTileToTrait(ETrait trait, Func<INeuronBoardController, ETrait, Hex> selector, int delay = 0) {
            var selectedHex = selector.Invoke(BoardController, trait);
            await AnimationManager.Register(AddTile(selectedHex, delay));
        }

        private async Task AddTile(Hex hex, int delay = 0) {
            await Task.Delay(delay);
            await BoardController.AddTile(hex);
        }

        #endregion

        private int GetEffectStrengthBasedOnNeurons(int neuronAmount) {
            // var frac = (float) neuronAmount / _neuronsController.CountNeurons;
            // return Mathf.RoundToInt(Mathf.SmoothStep(0, maxEffectStrength, frac));
            return Mathf.Clamp(Mathf.RoundToInt(effectScale * Mathf.Log(neuronAmount + 1)), 0, maxEffectStrength);
        }

        public Hex RandomEmptyHexSelector(INeuronBoardController boardController, ETrait trait) {
            var edgeHexes = BoardController.Manipulator
                .GetEdge(TraitAccessor.TraitToDirection(trait));
            var surroundingHexes = BoardController.Manipulator.GetSurroundingHexes(edgeHexes, true);
            var onlyEmptySurroundingHexes = surroundingHexes.Where(h => !BoardController.Board.HasPosition(h));
            var onlyContainedInTrait = onlyEmptySurroundingHexes.Where(h =>
                    TraitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(h)) == trait)
                .ToArray();
            return onlyContainedInTrait[Random.Range(0, onlyContainedInTrait.Length)];
        }

        public Hex MaxNeighborEmptyHexSelector(INeuronBoardController boardController, ETrait trait) {
            var edgeHexes = BoardController.Manipulator
                .GetEdge(TraitAccessor.TraitToDirection(trait));
            var surroundingHexes = BoardController.Manipulator.GetSurroundingHexes(edgeHexes, true);
            var onlyEmptySurroundingHexes = surroundingHexes.Where(h => !BoardController.Board.HasPosition(h));
            var onlyContainedInTrait = onlyEmptySurroundingHexes.Where(h =>
                    TraitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(h)) == trait)
                .ToArray();
            // give priority to tiles with neighbors in order to promote island connection
            var orderedByExistingNeighbors = onlyContainedInTrait
                 .OrderByDescending(h =>
                     BoardManipulationOddR<IBoardNeuron>.GetNeighboursStatic(h)
                         .Count(n => BoardController.Board.HasPosition(n)))
                 .ToArray();
            return orderedByExistingNeighbors[0];

        }
    }
}