using System.Linq;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons;
using Main.StoryPoints.Interfaces;
using Main.StoryPoints.SPProviders;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.StoryPoints {
    public class MStoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager neuronEventManager;

        public string Description => _spData.description;
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _spData.reward;
        public DecidingTraits DecidingTraits => _spData.decidingTraits;
        public TraitDecisionEffects DecisionEffects { get; private set; }
        public bool Evaluated { get; private set; } = false;

        
        private StoryPointData _spData;
        

        public void InitData(StoryPointData spData) {
            // set data
            _spData = spData;
            TurnsToEvaluation = spData.turnsToEvaluation;
            
            // notify
            storyEventManager.Raise(StoryEvents.OnInitStory, new StoryEventArgs(this));
        }

        public void Destroy() {
            Destroy(gameObject);
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

        public void Evaluate(IBoardNeuronsController controller) {
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
            
            var maxTraits = controller.GetMaxTrait(DecidingTraits.Keys).ToArray();
            var maxTrait = maxTraits[Random.Range(0, maxTraits.Length - 1)];

            DecisionEffects = new TraitDecisionEffects {
                DecidingTrait = maxTrait,
                Outcome = DecidingTraits[maxTrait].Outcome,
                BoardEffect = DecidingTraits[maxTrait].BoardEffect
            };
            Evaluated = true;

            // dispatch events
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(Reward));

        }
    }
}