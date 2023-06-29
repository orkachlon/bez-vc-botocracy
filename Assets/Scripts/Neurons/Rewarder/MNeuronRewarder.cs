﻿using System;
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

        [SerializeField] private int minReward, maxReward;
        
        protected ITraitAccessor _traitAccessor;
        
        [Header("Event Managers"), SerializeField]
        protected SEventManager boardEventManager;
        [SerializeField] protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager neuronEventManager;

        protected readonly Dictionary<Hex, int> _rewardHexes = new Dictionary<Hex, int>();
        

        #region UnityMethods

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
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

        #endregion

        #region EVentHandlers

        protected virtual void PickRewardTilesRandomly(EventArgs obj) {
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                // each trait has a separate chance to get a reward tile
                var currentAmount = _rewardHexes.Keys.Count(h => trait.Equals(_traitAccessor.HexToTrait(h)));
                var rewardTileChance = 1f / (1 + currentAmount);
                if (rewardTileChance < Random.value) {
                    continue;
                }

                var rewardPossibleTiles = _traitAccessor.GetTraitEdgeHexes(trait);     
                if (rewardPossibleTiles.Length == 0) {
                    continue;
                }
                var emptyTiles = _traitAccessor.GetTraitEmptyHexes(trait, rewardPossibleTiles);
                if (emptyTiles.Length == 0 && currentAmount == 0) {
                    // try to use any empty tile
                    emptyTiles = _traitAccessor.GetTraitEmptyHexes(trait);
                }
                if (emptyTiles.Length == 0) {
                    continue;
                }


                var randomEmptyTile = emptyTiles[Random.Range(0, emptyTiles.Length)];
                _rewardHexes[randomEmptyTile] = Random.Range(minReward, maxReward);
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile));
            }
        }

        protected void CheckForRewardTiles(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> addEventArgs) {
                return;
            }

            var hex = addEventArgs.Hex;
            if (!_rewardHexes.ContainsKey(hex)) {
                return;
            }
            // reward neurons!
            neuronEventManager.Raise(NeuronEvents.OnRewardTileReached, new RewardTileArgs(hex));
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(_rewardHexes[hex]));
            _rewardHexes.Remove(hex);
        }

        private void OnTileRemoved(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs tileArgs) {
                return;
            }

            if (_rewardHexes.ContainsKey(tileArgs.Hex)) {
                _rewardHexes.Remove(tileArgs.Hex);
            }
        }

        // might not need this if i have trait accessor
        // private void UpdateEmptyTiles(EventArgs obj) {
        //     
        // }

        #endregion
    }
}