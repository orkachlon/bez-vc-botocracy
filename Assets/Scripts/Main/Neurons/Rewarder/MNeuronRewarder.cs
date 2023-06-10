using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.StoryPoints;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Neurons.Rewarder {
    [RequireComponent(typeof(ITraitAccessor))]
    public class MNeuronRewarder : MonoBehaviour {

        [SerializeField] private int minReward, maxReward;
        
        private ITraitAccessor _traitAccessor;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        private readonly Dictionary<Hex, int> _rewardHexes = new Dictionary<Hex, int>();
        

        #region UnityMethods

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            // boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateEmptyTiles);
            boardEventManager.Register(ExternalBoardEvents.OnAddElement, CheckForRewardTiles);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            storyEventManager.Register(StoryEvents.OnInitStory, PickRewardTilesRandomly);
        }

        private void OnDisable() {
            // boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateEmptyTiles);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddElement, CheckForRewardTiles);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnTileRemoved);
            storyEventManager.Unregister(StoryEvents.OnInitStory, PickRewardTilesRandomly);
        }

        #endregion

        #region EVentHandlers

        private void PickRewardTilesRandomly(EventArgs obj) {
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
                if (emptyTiles.Length == 0) {
                    return;
                }

                var randomEmptyTile = emptyTiles[Random.Range(0, emptyTiles.Length)];
                _rewardHexes[randomEmptyTile] = Random.Range(minReward, maxReward);
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile));
            }
        }

        private void CheckForRewardTiles(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> addEventArgs) {
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