using System.Linq;
using Core.EventSystem;
using Main.GameStats;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons;
using Main.StoryPoints.SPProviders;
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

        public string StoryDescription => _spData.description;
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _spData.reward;
        public string Outcome { get; private set; }
        public StatToTraitWeights TraitWeights { get; private set; }

        public bool Evaluated { get; private set; } = false;

        private StoryPointData _spData;
        

        public void InitData(StoryPointData spData) {
            // set data
            _spData = spData;
            TurnsToEvaluation = spData.turnsToEvaluation;
            TraitWeights = spData.statEffects;
            
            // for legibility file values are in [-1,1]. We map them here to [-0.5,0.5].
            TraitWeights.Keys.ToList().ForEach(s =>
                TraitWeights[s].Keys.ToList().ForEach(t => TraitWeights[s][t] = TraitWeights[s][t] * 0.5f));
            
            // notify
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
            foreach (var stat in TraitWeights.Keys) {
                // calculation is: sum(traitWeight * curve(numNeuronsPerTrait / neuronsOnGrid)) / numTraits
                var neuronEvaluation = 0f;
                foreach (var trait in TraitWeights[stat].Keys) {
                    var fraction = (float) controller.GetTraitCount(trait) / (controller.CountNeurons - 1); // don't count starting neuron
                    var evaluationValue = neuronEvaluationWeight.Evaluate(fraction);
                    neuronEvaluation += TraitWeights[stat][trait] * evaluationValue;
                }
                var contributionAmount = neuronEvaluation / EnumUtil.GetValues<ETraitType>().Count();
                statEventManager.Raise(StatEvents.OnContributeToStat, new StatContributeEventArgs(stat, contributionAmount));
            }
            
            // set outcome
            var max = 0;
            var maxTrait = ETraitType.Commander;
            foreach (var trait in _spData.outcomes.Keys) {
                if (max >= controller.GetTraitCount(trait)) {
                    continue;
                }
                max = controller.GetTraitCount(trait);
                maxTrait = trait;
            }
            Outcome = _spData.outcomes[maxTrait];
            Evaluated = true;

            // dispatch events
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(Reward));

        }
    }
}