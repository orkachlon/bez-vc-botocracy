using System.Linq;
using Core.EventSystem;
using Main.GameStats;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons;
using Main.Traits;
using Main.Utils;
using TMPro;
using UnityEngine;

namespace Main.StoryPoints {
    public class StoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager statEventManager;

        public string StoryDescription { get; private set; }
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _reward;
        public bool Evaluated { get; private set; } = false;

        private int _reward;
        private StatToTraitWeights _calculationDict;
        

        public void InitData(string eventDescription, int reward, int turnsToEvaluation, StatToTraitWeights calculationDict) {
            // set data
            StoryDescription = eventDescription;
            TurnsToEvaluation = turnsToEvaluation;
            _reward = reward;
            _calculationDict = calculationDict;
            
            // for legibility file values are in [-1,1]. We map them here to [-0.5,0.5].
            _calculationDict.Keys.ToList().ForEach(s =>
                _calculationDict[s].Keys.ToList().ForEach(t => _calculationDict[s][t] = _calculationDict[s][t] * 0.5f));
            
            // set visual elements
            storyEventManager.Raise(StoryEvents.OnInitStory, new StoryEventArgs(this));
        }

        public void Decrement() {
#if UNITY_EDITOR
            if (TurnsToEvaluation == 0) {
                Debug.Log("Event turn counter < 0 !!!");
            }
#endif
            TurnsToEvaluation -= 1;
            storyEventManager.Raise(StoryEvents.OnDecrement, new StoryEventArgs(this));
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
    }
}