using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.Neuron;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neurons.Rewarder {
    [RequireComponent(typeof(ITraitAccessor))]
    public class MNeuronRewarder : MonoBehaviour {

        [SerializeField] private AnimationCurve rewardAmountDistribution;
        
        protected ITraitAccessor TraitAccessor;
        
        [Header("Event Managers"), SerializeField]
        protected SEventManager boardEventManager;
        [SerializeField] protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager neuronEventManager;

        protected readonly Dictionary<Hex, int> RewardHexes = new();
        

        #region UnityMethods

        private void Awake() {
            TraitAccessor = GetComponent<ITraitAccessor>();
        }

        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, PickRewardTilesRandomly);
            boardEventManager.Register(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, PickRewardTilesRandomly);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, PickRewardTilesRandomly);
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, PickRewardTilesRandomly);
        }

        private void OnDestroy() {
            RewardHexes.Clear();
        }

        #endregion

        #region EVentHandlers

        protected virtual void PickRewardTilesRandomly(EventArgs obj) {
            RemoveOutOfBoundsRewardTiles();
            
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                // each trait has a separate chance to get a reward tile
                var currentAmount = RewardHexes.Keys.Count(h => trait.Equals(TraitAccessor.HexToTrait(h)));
                var rewardTileChance = 1f / (1 + currentAmount);
                if (rewardTileChance < Random.value) {
                    continue;
                }

                var rewardPossibleTiles = TraitAccessor.GetTraitEdgeHexes(trait);     
                if (rewardPossibleTiles.Length == 0) {
                    continue;
                }
                var emptyTiles = TraitAccessor.GetTraitEmptyHexes(trait, rewardPossibleTiles);
                if (emptyTiles.Length == 0 && currentAmount == 0) {
                    // try to use any empty tile
                    emptyTiles = TraitAccessor.GetTraitEmptyHexes(trait);
                }
                if (emptyTiles.Length == 0) {
                    continue;
                }


                var randomEmptyTile = emptyTiles[Random.Range(0, emptyTiles.Length)];
                RewardHexes[randomEmptyTile] = GetRewardAmount();
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile, RewardHexes[randomEmptyTile]));
            }
        }

        private int GetRewardAmount() {
            return Mathf.RoundToInt(rewardAmountDistribution.Evaluate(Random.value));
        }

        private void RemoveOutOfBoundsRewardTiles() {
            foreach (var hex in RewardHexes.Keys) {
                if (TraitAccessor.HexToTrait(hex).HasValue) {
                    continue;
                }
                neuronEventManager.Raise(NeuronEvents.OnRewardTileRemoved, new RewardTileArgs(hex, -1));
                RewardHexes.Remove(hex);
            }
        }

        protected void CheckForRewardTiles(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> addEventArgs) {
                return;
            }

            var hex = addEventArgs.Hex;
            if (!RewardHexes.ContainsKey(hex)) {
                return;
            }
            // reward neurons!
            neuronEventManager.Raise(NeuronEvents.OnRewardTileReached, new RewardTileArgs(hex, RewardHexes[hex]));
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(RewardHexes[hex]));
            RewardHexes.Remove(hex);
        }

        private void OnTileRemoved(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs tileArgs) {
                return;
            }

            if (!RewardHexes.ContainsKey(tileArgs.Hex)) {
                return;
            }
            RewardHexes.Remove(tileArgs.Hex);
            neuronEventManager.Raise(NeuronEvents.OnRewardTileRemoved, new RewardTileArgs(tileArgs.Hex, -1));
        }
        
        #endregion
    }
}