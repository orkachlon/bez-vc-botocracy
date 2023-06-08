using System;
using Core.EventSystem;
using Core.Utils;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.Neurons.Rewarder {
    public class MUINeuronRewarder : MonoBehaviour {

        [SerializeField] private TileBase rewardTile;

        [SerializeField] private MNeuronBoardController boardController;

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;


        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Register(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, RemoveRewardTile);
        }

        private void RemoveRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }

            var hexDirection = boardController.Manipulator.GetDirection(rewardArgs.RewardHex);
            if (!hexDirection.HasValue) {
                MLogger.LogEditor($"Reward hex {rewardArgs.RewardHex} is out of board bounds!");
                return;
            }

            var trait = ITraitAccessor.DirectionToTrait(hexDirection.Value);
            if (!trait.HasValue) {
                MLogger.LogEditor($"Trait not found for hex {rewardArgs.RewardHex}");
                return;
            }
            boardController.SetTile(rewardArgs.RewardHex, boardController.GetTraitTileBase(trait.Value));
        }

        private void PlaceRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            boardController.SetTile(rewardArgs.RewardHex, rewardTile);
        }
    }
}