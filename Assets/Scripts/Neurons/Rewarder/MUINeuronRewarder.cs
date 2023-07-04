using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;

namespace Neurons.Rewarder {
    public class MUINeuronRewarder : MonoBehaviour {

        [SerializeField] private MRewardEffect rewardPrefab;

        [SerializeField] private MNeuronBoardController boardController;

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        private readonly Dictionary<Hex, MRewardEffect> _rewardAnimationSeqs = new();
        
        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Register(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
            neuronEventManager.Register(NeuronEvents.OnRewardTileRemoved, RemoveRewardTile);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileRemoved, RemoveRewardTile);

            KillAnimations();
        }

        private void OnDestroy() {
            KillAnimations();
        }

        private void KillAnimations() {
            foreach (var effect in _rewardAnimationSeqs.Values.Where(e => e != null)) {
                effect.Complete();
                effect.Kill();
                Destroy(effect.gameObject);
            }
            _rewardAnimationSeqs.Clear();
        }

        private void RemoveRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            KillAndUnregisterEffect(rewardArgs.RewardHex);            
            boardController.SetTile(rewardArgs.RewardHex, null, BoardConstants.RewardTilemapLayer);
        }

        private async void PlayRewardEffect(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            var hex = rewardArgs.RewardHex;
            var amount = rewardArgs.Amount;
            
            await _rewardAnimationSeqs[hex].PlayReward(boardController, hex, amount);
            
            KillAndUnregisterEffect(rewardArgs.RewardHex);
        }

        private void KillAndUnregisterEffect(Hex hex) {
            _rewardAnimationSeqs[hex]?.Complete();
            _rewardAnimationSeqs[hex]?.Kill();
            Destroy(_rewardAnimationSeqs[hex].gameObject);
            _rewardAnimationSeqs.Remove(hex);
        }

        private void PlaceRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }

            var hex = rewardArgs.RewardHex;
            var effect = Instantiate(rewardPrefab, boardController.HexToWorldPos(rewardArgs.RewardHex), Quaternion.identity, transform);
            effect.PlayIdle(boardController, hex);
            _rewardAnimationSeqs[hex] = effect;
        }
    }
}