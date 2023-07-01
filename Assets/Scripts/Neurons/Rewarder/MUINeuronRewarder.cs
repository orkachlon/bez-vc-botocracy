using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using Types.Hex.Coordinates;
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

        private readonly Dictionary<Hex, Sequence> _rewardAnimationSeqs = new();

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Register(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
            neuronEventManager.Register(NeuronEvents.OnRewardTileRemoved, RemoveRewardTile);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardTilePicked, PlaceRewardTile);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileReached, PlayRewardEffect);
            neuronEventManager.Unregister(NeuronEvents.OnRewardTileRemoved, RemoveRewardTile);
        }

        // tiles are automatically changed to pressed tiles when a neuron is placed on them.
        // no need to change them from here.
        private void RemoveRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            
            boardController.SetTile(rewardArgs.RewardHex, null, BoardConstants.RewardTilemapLayer);
        }

        private async void PlayRewardEffect(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }
            var hex = rewardArgs.RewardHex;
            _rewardAnimationSeqs[hex]?.Complete();
            _rewardAnimationSeqs[hex]?.Kill();

            var startColor = boardController.GetColor(hex, BoardConstants.RewardTilemapLayer);
            DOVirtual.Color(startColor, Color.white, rewardAnimationDuration * 0.3f,
                c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer));
            
            var amount = rewardArgs.Amount;
            var effectsTasks = new List<Task>();
            for (var i = 0; i < amount; i++) {
                var effect = Instantiate(rewardPrefab, boardController.HexToWorldPos(rewardArgs.RewardHex), Quaternion.identity);
                var effectTask = DOTween.Sequence()
                    .AppendInterval(i * rewardAnimationDuration * 0.5f)
                    .Append(effect.transform.DOMoveY(effect.transform.position.y + rewardAnimationHeight, rewardAnimationDuration))
                    .Join(effect.DOFade(0, rewardAnimationDuration))
                    .OnComplete(() => Destroy(effect.gameObject))
                    .AsyncWaitForCompletion();
                effectsTasks.Add(effectTask);
            }

            await Task.WhenAll(effectsTasks);

            DOTween.Sequence()
                .Append(DOVirtual.Color(Color.white, Color.clear, rewardAnimationDuration * 0.5f,
                    c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer)))
                .OnComplete(() => boardController.SetTile(hex, null, BoardConstants.RewardTilemapLayer));

        }

        private void PlaceRewardTile(EventArgs obj) {
            if (obj is not RewardTileArgs rewardArgs) {
                return;
            }

            var hex = rewardArgs.RewardHex;
            boardController.SetTile(hex, rewardTile, BoardConstants.RewardTilemapLayer);
            boardController.SetColor(hex, Color.clear, BoardConstants.RewardTilemapLayer);
            var hexSeq = DOTween.Sequence()
                .Append(DOVirtual.Color(new Color(1, 1, 1, 0f), new Color(1, 1, 1, 0.7f), rewardAnimationDuration * 5,
                    c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer)))
                .Append(DOVirtual.Color(new Color(1, 1, 1, 0.7f), new Color(1, 1, 1, 0f), rewardAnimationDuration * 5,
                    c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer)))
                .SetLoops(-1, LoopType.Restart);
            _rewardAnimationSeqs[hex] = hexSeq;
        }
    }
}