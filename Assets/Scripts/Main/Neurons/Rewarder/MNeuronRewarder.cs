using System;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
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
            storyEventManager.Register(StoryEvents.OnInitStory, PickRewardTilesRandomly);
            neuronEventManager.Register(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        private void OnDisable() {
            // boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateEmptyTiles);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddElement, CheckForRewardTiles);
            storyEventManager.Unregister(StoryEvents.OnInitStory, PickRewardTilesRandomly);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        #endregion

        #region EVentHandlers

        private void PickRewardTilesRandomly(EventArgs obj) {
            var currentAmount = _rewardHexes.Count;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                var rewardTileChance = 1f / (1 + currentAmount);
                if (rewardTileChance < Random.value) {
                    continue;
                }

                var emptyTiles = _traitAccessor.GetEmptyTiles(trait);
                if (emptyTiles.Length == 0) {
                    continue;
                }

                var randomEmptyTile = emptyTiles[Random.Range(0, emptyTiles.Length)];
                _rewardHexes[randomEmptyTile] = Random.Range(minReward, maxReward);
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile));
            }
        }

        private void CheckForRewardTiles(EventArgs obj) {
            if (obj is not OnPlaceElementEventArgs<BoardNeuron> addEventArgs) {
                return;
            }

            var hex = addEventArgs.Hex;
            if (!_rewardHexes.ContainsKey(hex)) {
                return;
            }
            // reward neurons!
            neuronEventManager.Raise(NeuronEvents.OnRewardTileReached, new RewardTileArgs(hex));
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(_rewardHexes[hex]));
        }

        private void RemoveRewardTile(EventArgs eventArgs) {
            if (eventArgs is not RewardTileArgs rewardArgs) {
                return;
            }

            _rewardHexes.Remove(rewardArgs.RewardHex);
        }

        // might not need this if i have trait accessor
        // private void UpdateEmptyTiles(EventArgs obj) {
        //     
        // }

        #endregion
    }
}