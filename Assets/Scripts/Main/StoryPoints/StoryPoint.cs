using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.GameStats;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons;
using Main.Traits;
using Main.Utils;
using TMPro;
using UnityEngine;

namespace Main.StoryPoints {
    public class StoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TextMeshPro description;
        [SerializeField] private TextMeshPro turnCounter;
        [SerializeField] private TextMeshPro rewardText;
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager statEventManager;

        public int TurnsToEvaluation { get; private set; }
        public bool Evaluated { get; private set; } = false;

        private int _reward;
        private StatToTraitWeights _calculationDict;

        private void Awake() {
            // throw new NotImplementedException();
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
            rewardText.text = $"Reward: {_reward}";
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
        public void Evaluate(IBoardNeuronController controller) {
            if (Evaluated) {
#if UNITY_EDITOR
                Debug.Log("Event already evaluated!");
                return;
#endif
            }
            // shouldn't happen because we always have the first neuron
            if (controller.CountNeurons == 0) {
                return;
            }
            foreach (var stat in _calculationDict.Keys) {
                // sum(traitWeight * curve(numNeuronsPerTrait / neuronsOnGrid)) / numTraits
                var neuronEvaluation = 0f;
                foreach (var trait in _calculationDict[stat].Keys) {
                    var fraction = (float) controller.GetTraitCount(trait) / controller.CountNeurons;
                    var evaluationValue = neuronEvaluationWeight.Evaluate(fraction);
                    neuronEvaluation += _calculationDict[stat][trait] * evaluationValue;
                }
                var numTraits = EnumUtil.GetValues<ETraitType>().Count();
                statEventManager.Raise(StatEvents.OnContributeToStat, new StatContributeEventArgs(stat, neuronEvaluation / numTraits));
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