using Events.Board;
using Events.Neuron;
using Neurons.Rewarder;
using System;
using System.Linq;
using Types.Hex.Coordinates;
using Random = UnityEngine.Random;

namespace Tutorial.Neurons {
    public class MTutorialNeuronRewarder : MNeuronRewarder {

        public int Count => RewardHexes.Count;

        protected override void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
        }

        protected override void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupied, CheckForRewardTiles);
        }

        protected override void PickRewardTilesRandomly(EventArgs obj) {
            foreach (var trait in TutorialConstants.Traits) {
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
                RewardHexes[randomEmptyTile] = 1;
                neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(randomEmptyTile, RewardHexes[randomEmptyTile]));
            }
        }

        public void SelectRewardHex(Hex hex, int rewardAmount) {
            RewardHexes[hex] = rewardAmount;
            neuronEventManager.Raise(NeuronEvents.OnRewardTilePicked, new RewardTileArgs(hex, RewardHexes[hex]));
        }

        public void RewardRandomTiles() {
            PickRewardTilesRandomly(null);
        }

        public void Clear() {
            RewardHexes.Clear();
        }
    }
}