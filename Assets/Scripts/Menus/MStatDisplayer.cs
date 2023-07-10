using System.Linq;
using DG.Tweening;
using Menus.EndGame;
using TMPro;
using Types.Trait;
using UnityEngine;

namespace Menus {
    public class MStatDisplayer : MonoBehaviour {
        
        [Header("Stats"), SerializeField] private TextMeshProUGUI neuronsConnected;
        [SerializeField] private TextMeshProUGUI dummiesSpawned;
        [SerializeField] private TextMeshProUGUI neuronsCulled;
        [SerializeField] private TextMeshProUGUI eventsSurvived;
        [SerializeField] private TextMeshProUGUI strongestTrait;
        [SerializeField] private TextMeshProUGUI weakestTrait;
        [SerializeField] private TextMeshProUGUI tilesAdded;
        [SerializeField] private TextMeshProUGUI tilesRemoved;
        
        public void AnimateStats(MStatCollector statProvider, float animationDuration = 0) {
            // neurons
            AnimateTextCount(neuronsConnected, 0, statProvider.NeuronsPlaced.Values.Sum(), animationDuration);
            AnimateTextCount(dummiesSpawned, 0, statProvider.DummiesSpawned, animationDuration);
            AnimateTextCount(neuronsCulled, 0, statProvider.NeuronsExploded, animationDuration);
            // SPs
            AnimateTextCount(eventsSurvived, 0, statProvider.SPCounter, animationDuration);
            // traits
            var strongest = statProvider.TraitCounter.IndexOf(statProvider.TraitCounter.Max());
            var weakest = statProvider.TraitCounter.IndexOf(statProvider.TraitCounter.Min());
            AnimateTextCount(strongestTrait, 0, strongest, animationDuration, i => strongestTrait.text = $"{(ETrait) i}");
            AnimateTextCount(weakestTrait, 0, weakest, animationDuration, i => weakestTrait.text = $"{(ETrait) i}");
            // tiles
            AnimateTextCount(tilesAdded, 0, statProvider.TilesAdded, animationDuration);
            AnimateTextCount(tilesRemoved, 0, statProvider.TilesRemoved, animationDuration);
        }

        private Tween AnimateTextCount(TextMeshProUGUI textField, int from, int to, float duration, TweenCallback<int> callback = null) {
            callback ??= i => textField.text = $"{i}";
            return DOVirtual.Int(from, to, duration, callback);
        }
    }
}