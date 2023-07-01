using System;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using DG.Tweening;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Neurons.Rewarder {
    public class MUINeuronRewarder : MonoBehaviour {

        [SerializeField] private TileBase rewardTile;
        [SerializeField] private SpriteRenderer rewardPrefab;
        [SerializeField] private float rewardAnimationHeight;

        [SerializeField] private MNeuronBoardController boardController;
        [SerializeField] private float rewardAnimationDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        private ITraitAccessor _traitAccessor;

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Register(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
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

        private async void PlayRewardEffect(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }

            var amount = rewardArgs.Amount;
            for (var i = 0; i < amount; i++) {
                var effect = Instantiate(rewardPrefab, boardController.HexToWorldPos(rewardArgs.RewardHex), Quaternion.identity);
                DOTween.Sequence()
                    .Insert(0, effect.transform.DOMoveY(effect.transform.position.y + rewardAnimationHeight, rewardAnimationDuration))
                    .Insert(0, effect.DOFade(0, rewardAnimationDuration))
                    .OnComplete(() => Destroy(effect.gameObject));
                await Task.Delay((int)(rewardAnimationDuration * 500));
            }
        }

        private void PlaceRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            boardController.SetTile(rewardArgs.RewardHex, rewardTile);
        }
    }
}