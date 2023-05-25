using System.Linq;
using Core.EventSystem;
using Main.GameStats;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.StoryPoints {
    public class MStoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager statEventManager;

        public string StoryDescription { get; private set; }
        public int TurnsToEvaluation { get; private set; }
        public int Reward { get; private set; }
        public string Outcome { get; private set; }
        public StatToTraitWeights TraitWeights => _calculationDict;
        public bool Evaluated { get; private set; } = false;

        private StatToTraitWeights _calculationDict;
        private TraitsToOutcomes _possibleOutcomes;
        

        public void InitData(string eventDescription, int reward, int turnsToEvaluation, TraitsToOutcomes outcomes, StatToTraitWeights calculationDict) {
            // set data
            StoryDescription = eventDescription;
            TurnsToEvaluation = turnsToEvaluation;
            Reward = reward;
            _possibleOutcomes = outcomes;
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
            TurnsToEvaluation--;
            storyEventManager.Raise(StoryEvents.OnDecrement, new StoryEventArgs(this));
        }

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
                // calculation is: sum(traitWeight * curve(numNeuronsPerTrait / neuronsOnGrid)) / numTraits
                var neuronEvaluation = 0f;
                foreach (var trait in _calculationDict[stat].Keys) {
                    var fraction = (float) controller.GetTraitCount(trait) / (controller.CountNeurons - 1); // don't count starting neuron
                    var evaluationValue = neuronEvaluationWeight.Evaluate(fraction);
                    neuronEvaluation += _calculationDict[stat][trait] * evaluationValue;
                }
                
                // set outcome
                var max = 0;
                ETraitType maxTrait = ETraitType.Commander;
                foreach (var trait in _possibleOutcomes.Keys) {
                    if (max >= controller.GetTraitCount(trait)) {
                        continue;
                    }
                    max = controller.GetTraitCount(trait);
                    maxTrait = trait;
                }

                Outcome = _possibleOutcomes[maxTrait];
                    
                var contributionAmount = neuronEvaluation / EnumUtil.GetValues<ETraitType>().Count();
                

                // dispatch events
                storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(this));
                statEventManager.Raise(StatEvents.OnContributeToStat, new StatContributeEventArgs(stat, contributionAmount));
                neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(Reward));
            }

            Evaluated = true;
        }
    }
}