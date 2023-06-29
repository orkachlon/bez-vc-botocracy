using System;
using Core.EventSystem;
using Core.Utils;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Neurons.Rewarder {
    public class MUINeuronRewarder : MonoBehaviour {

        [SerializeField] private TileBase rewardTile;

        [SerializeField] private MNeuronBoardController boardController;

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        private ITraitAccessor _traitAccessor;

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            // neuronEventManager.Register(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            // neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        // tiles are automatically changed to pressed tiles when a neuron is placed on them.
        // no need to change them from here.
        private void RemoveRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }

            var hexDirection = boardController.Manipulator.GetDirection(rewardArgs.RewardHex);
            if (!hexDirection.HasValue) {
                MLogger.LogEditor($"Reward hex {rewardArgs.RewardHex} is out of board bounds!");
                return;
            }

            var trait = _traitAccessor.DirectionToTrait(hexDirection.Value);
            if (!trait.HasValue) {
                MLogger.LogEditor($"Trait not found for hex {rewardArgs.RewardHex}");
                return;
            }
            // boardController.SetTile(rewardArgs.RewardHex, boardController.GetTraitTileBase(trait.Value));
        }

        private void PlaceRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            boardController.SetTile(rewardArgs.RewardHex, rewardTile);
        }
    }
}