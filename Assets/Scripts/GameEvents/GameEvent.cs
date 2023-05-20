using System;
using System.Linq;
using Core.EventSystem;
using Managers;
using Neurons;
using TMPro;
using Traits;
using UnityEngine;
using Utils;
using Grid = Grids.Grid;

namespace GameEvents {
    public class GameEvent : MonoBehaviour, IGameEvent {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TextMeshPro description;
        [SerializeField] private TextMeshPro turnCounter;
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;

        public int TurnsToEvaluation { get; private set; }
        public bool Evaluated { get; private set; } = false;

        private int _reward;
        private StatToTraitWeights _calculationDict;

        private void Awake() {
            throw new NotImplementedException();
        }

        public void InitData(string eventDescription, int reward, int turnsToEvaluation, StatToTraitWeights calculationDict) {
            // set elementData
            TurnsToEvaluation = turnsToEvaluation;
            _reward = reward;
            _calculationDict = calculationDict;
            // for legibility file values are in [-1,1]. We map them here to [-0.5,0.5].
            _calculationDict.Keys.ToList().ForEach(s =>
                _calculationDict[s].Keys.ToList().ForEach(t => _calculationDict[s][t] = _calculationDict[s][t] * 0.5f));
            
            // set visual elements
            description.text = eventDescription;
            UpdateTurnCounter();
        }

        public void Hide() {
            sprite.enabled = false;
            description.enabled = false;
        }

        public void Decrement() {
#if UNITY_EDITOR
            if (TurnsToEvaluation == 0) {
                Debug.Log("Event turn counter < 0 !!!");
            }
#endif
            TurnsToEvaluation -= 1;
            UpdateTurnCounter();
        }

        // maybe this function should return a dict<Stat, contributionAmount> instead of calling the StatManager itself
        public void Evaluate() {
            if (Evaluated) {
#if UNITY_EDITOR
                Debug.Log("Event already evaluated!");
                return;
#endif
            }
            // shouldn't happen because we always have the first neuron
            if (Grid.Instance.CountNeurons() == 0) {
                return;
            }
            foreach (var stat in _calculationDict.Keys) {
                // sum(traitWeight * curve(numNeuronsPerTrait / neuronsOnGrid)) / numTraits
                var neuronEvaluation = _calculationDict[stat].Keys.Sum(trait => _calculationDict[stat][trait] * neuronEvaluationWeight.Evaluate((float) Grid.Instance.CountNeurons(trait) / Grid.Instance.CountNeurons()));
                var numTraits = EnumUtil.GetValues<ETraitType>().Count();
                StatManager.Instance.Contribute(stat, neuronEvaluation / numTraits);
                neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(_reward));
            }

            Evaluated = true;
        }

        #region VisualElements

        private void UpdateTurnCounter() {
            turnCounter.text = $"Turns: {TurnsToEvaluation}";
        }

        #endregion
    }
}