using System;
using System.Linq;
using Managers;
using TMPro;
using Traits;
using UnityEngine;
using Utils;
using Grid = Grids.Grid;

namespace GameEvents {
    public class GameEvent : MonoBehaviour, IGameEvent {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TextMeshPro description;
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        private int _reward;
        private StatToTraitWeights _calculationDict;

        public void InitData(string eventDescription, int reward, StatToTraitWeights calculationDict) {
            description.text = eventDescription;
            _reward = reward;
            _calculationDict = calculationDict;
            // for legibility file values are in [-1,1]. We map them here to [-0.5,0.5].
            _calculationDict.Keys.ToList().ForEach(s =>
                _calculationDict[s].Keys.ToList().ForEach(t => _calculationDict[s][t] = _calculationDict[s][t] * 0.5f));
        }

        public void Hide() {
            sprite.enabled = false;
            description.enabled = false;
        }

        // maybe this function should return a dict<Stat, contributionAmount> instead of calling the StatManager itself
        public void Evaluate() {
            foreach (var stat in _calculationDict.Keys) {
                // sum(traitWeight * curve(numNeuronsPerTrait / neuronsOnGrid)) / numTraits
                var neuronEvaluation = _calculationDict[stat].Keys.Sum(trait => _calculationDict[stat][trait] * neuronEvaluationWeight.Evaluate((float) Grid.Instance.CountNeurons(trait) / Grid.Instance.CountNeurons()));
                var numTraits = EnumUtil.GetValues<ETraitType>().Count();
                StatManager.Instance.Contribute(stat, neuronEvaluation / numTraits);
            }
        }
    }
}