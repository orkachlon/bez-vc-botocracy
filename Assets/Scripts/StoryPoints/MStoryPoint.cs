﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.General;
using Events.SP;
using StoryPoints.UI;
using Types.Board;
using Types.GameState;
using Types.StoryPoint;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StoryPoints {
    public class MStoryPoint : MonoBehaviour, IStoryPoint {
        
        [SerializeField] private AnimationCurve neuronEvaluationWeight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager gmEventManager;

        public int Id => _spData.id;
        public string Title => _spData.title;
        public string Description => _spData.description;
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _spData.reward;
        public DecidingTraits DecidingTraits => _spData.decidingTraits;
        public Sprite Artwork => _spData.image;
        public TraitDecisionEffects DecisionEffects { get; private set; }
        public bool Evaluated { get; private set; } = false;

        
        private StoryPointData _spData;
        private MUIStoryPoint _uiSP;
        private IBoardNeuronsController _neuronsController;

        #region UnityMethods

        private void Awake() {
            _uiSP = GetComponent<MUIStoryPoint>();
        }

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
            if (obj is not GameStateEventArgs {State: EGameState.StoryTurn}) {
                return;
            }

            HandleStoryTurn();
        }

        private void UpdateBoardState(EventArgs obj) {
            if (obj is not BoardStateEventArgs boardArgs) {
                return;
            }

            _neuronsController = boardArgs.ElementsController;
        }

        #endregion

        public void InitData(StoryPointData spData) {
            // set data
            _spData = spData;
            TurnsToEvaluation = spData.turnsToEvaluation;
            
            _uiSP.InitSPUI();
            // notify
            storyEventManager.Raise(StoryEvents.OnInitStory, new StoryEventArgs(this));
        }

        public async Task AwaitInitAnimation() {
            await _uiSP.PlayInitAnimation();
        }

        public async Task AwaitRemoveAnimation() {
            await _uiSP.PlayEvaluateAnimation();
        }

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

        private void Decrement() {
            if (TurnsToEvaluation == 0) {
                MLogger.LogEditor("Event turn counter < 0 !!!");
            }
            TurnsToEvaluation--;
            _uiSP.UpdateTurnCounter(TurnsToEvaluation);
            storyEventManager.Raise(StoryEvents.OnDecrement, new StoryEventArgs(this));
        }

        private async void Evaluate() {
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

            DecisionEffects = DecidingTraits[maxTrait];
            Evaluated = true;
            await AwaitRemoveAnimation();
        }
    }
}