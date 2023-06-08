using System;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.Events;
using Main.StoryPoints.Interfaces;
using Main.StoryPoints.SPProviders;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.StoryPoints {
    public class MStoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager gmEventManager;

        public string Description => _spData.description;
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _spData.reward;
        public DecidingTraits DecidingTraits => _spData.decidingTraits;
        public TraitDecisionEffects DecisionEffects { get; private set; }
        public bool Evaluated { get; private set; } = false;

        
        private StoryPointData _spData;
        private IBoardNeuronsController _neuronsController;

        #region UnityMethods

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateBoardState);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateBoardState);
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        #endregion
        
        #region EventHandlers

        private void OnAfterGameState(EventArgs obj) {
            if (obj is not GameStateEventArgs {State: GameState.StoryTurn}) {
                return;
            }

            HandleStoryTurn();
        }

        private void UpdateBoardState(EventArgs obj) {
            if (obj is not OnBoardStateBroadcastEventArgs boardArgs) {
                return;
            }

            _neuronsController = boardArgs.ElementsController;
        }

        #endregion

        private void HandleStoryTurn() {
            Decrement();
            if (TurnsToEvaluation > 0) {
                return;
            }

            if (_neuronsController == null) {
                MLogger.LogEditor("Couldn't evaluate SP: board state dependency wasn't injected");
                return;
            }
            Evaluate();
        }

        public void InitData(StoryPointData spData) {
            // set data
            _spData = spData;
            TurnsToEvaluation = spData.turnsToEvaluation;
            
            // notify
            storyEventManager.Raise(StoryEvents.OnInitStory, new StoryEventArgs(this));
        }

        private void Decrement() {
            if (TurnsToEvaluation == 0) {
                MLogger.LogEditor("Event turn counter < 0 !!!");
            }
            TurnsToEvaluation--;
            storyEventManager.Raise(StoryEvents.OnDecrement, new StoryEventArgs(this));
        }

        private void Evaluate() {
            if (Evaluated) {
                MLogger.LogEditor("Event already evaluated!");
                return;
            }
            // shouldn't happen because we always have the first neuron
            if (_neuronsController.CountNeurons == 0) {
                return;
            }
            
            var maxTraits = _neuronsController.GetMaxTrait(DecidingTraits.Keys).ToArray();
            var maxTrait = maxTraits[Random.Range(0, maxTraits.Length - 1)];

            DecisionEffects = new TraitDecisionEffects {
                DecidingTrait = maxTrait,
                Outcome = DecidingTraits[maxTrait].Outcome,
                BoardEffect = DecidingTraits[maxTrait].BoardEffect
            };
            Evaluated = true;
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(this));
            
            // NEURON REWARDS HAS BEEN MOVED TO MNeuronRewarder.cs
            // neuron reward
            // var neuronReward = 0;
            // foreach (var (trait, effect) in DecidingTraits[maxTrait].BoardEffect) {
            //     neuronReward += controller.GetTraitCount(trait) * effect;
            // }

            // dispatch events
            // neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(neuronReward));

        }
    }
}