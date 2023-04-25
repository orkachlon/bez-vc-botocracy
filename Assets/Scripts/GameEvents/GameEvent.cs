using System;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using Grid = Grids.Grid;

namespace GameEvents {
    public class GameEvent : MonoBehaviour, IGameEvent {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TextMeshPro description;

        private int _reward;
        private StatToTraitWeights _calculationDict;

        public void InitData(string eventDescription, int reward, StatToTraitWeights calculationDict) {
            description.text = eventDescription;
            _reward = reward;
            _calculationDict = calculationDict;
        }

        public void Hide() {
            sprite.enabled = false;
            description.enabled = false;
        }

        // maybe this function should return a dict<Stat, contributionAmount> instead of calling the StatManager itself
        public void Evaluate() {
            foreach (var stat in _calculationDict.Keys) {
                // sum the influence of all traits on this stat
                var contributionAmount = _calculationDict[stat].Keys.Sum(trait => Grid.Instance.CountNeuronsNormalized(trait) * _calculationDict[stat][trait]);
                StatManager.Instance.Contribute(stat, contributionAmount);
            }
        }
    }
}