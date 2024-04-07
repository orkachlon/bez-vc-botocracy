using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.General;
using Events.SP;
using StoryPoints.Interfaces;
using Types.Board;
using Types.GameState;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StoryPoints {
    public class MStoryPoint : MonoBehaviour, IStoryPoint {
        
        [Header("Event Managers"), SerializeField]
        protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager boardEventManager;
        [SerializeField] protected SEventManager gmEventManager;

        public int Id => _spData.Id;
        public string Title => _spData.Title;
        public string Description => _spData.Description;
        public int TurnsToEvaluation { get; private set; }
        public int Reward => _spData.Reward;
        public IDictionary<ETrait, ITraitDecisionEffects> DecidingTraits => _spData.DecidingTraits;
        public Sprite Artwork => _spData.Image;
        public ITraitDecisionEffects DecisionEffects { get; private set; }
        public bool Evaluated { get; private set; } = false;

        
        protected IUIStoryPoint UISP;
        private IStoryPointData _spData;
        private IBoardNeuronsController _neuronsController;

        #region UnityMethods

        private void Awake() {
            UISP = GetComponent<IUIStoryPoint>();
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

        protected void OnAfterGameState(EventArgs obj) {
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

        public void InitData(IStoryPointData spData) {
            // set data
            _spData = spData;
            TurnsToEvaluation = spData.TurnsToEvaluation;
            
            UISP.InitSPUI();
            // notify
            storyEventManager.Raise(StoryEvents.OnInitStory, new StoryEventArgs(this));
        }

        public async Task AwaitInitAnimation() {
            await UISP.PlayInitAnimation();
        }

        public async Task AwaitRemoveAnimation() {
            await UISP.PlayEvaluateAnimation();
        }

        protected virtual async void HandleStoryTurn() {
            await Decrement();
            if (TurnsToEvaluation > 0) {
                return;
            }

            if (_neuronsController == null) {
                MLogger.LogEditor("Couldn't evaluate SP: board state dependency wasn't injected");
                return;
            }
            Evaluate();
        }

        private async Task Decrement() {
            if (TurnsToEvaluation == 0) {
                MLogger.LogEditor("Event turn counter < 0 !!!");
            }
            TurnsToEvaluation--;
            UISP.UpdateTurnCounter(TurnsToEvaluation);
            if (TurnsToEvaluation > 0) {
                await UISP.PlayDecrementAnimation();
            }
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
            storyEventManager.Raise(StoryEvents.OnBeforeEvaluate, new StoryEventArgs(this));
            await AwaitRemoveAnimation();
        }

        public void RegisterOutcome(ISPProvider spProvider) {
            if (!string.IsNullOrEmpty(DecisionEffects.OutcomeModification)) {
                OutcomeModificationParser.ModifyOutcomes(spProvider, DecisionEffects.OutcomeModification);
            }
            spProvider.AddOutcome(DecisionEffects.OutcomeID);
        }
    }
}