using Events.Board;
using Events.Neuron;
using Neurons.Rewarder;
using System;
using System.Linq;
using Tutorial;
using Types.Hex.Coordinates;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tutorial.Neurons {
    public class MTutorialNeuronRewarder : MNeuronRewarder {

        public int Count => _rewardHexes.Count;

        protected override void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
        }

        protected override void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
        }

        protected override void PickRewardTilesRandomly(EventArgs obj) {
            foreach (var trait in TutorialConstants.Traits) {
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
                _rewardHexes[randomEmptyTile] = 1;
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile));
            }
        }

        public void SelectRewardHex(Hex hex, int rewardAmount) {
            _rewardHexes[hex] = rewardAmount;
            neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(hex));
        }

        public void RewardRandomTiles() {
            PickRewardTilesRandomly(null);
        }

        public void Clear() {
            _rewardHexes.Clear();
        }
    }
}