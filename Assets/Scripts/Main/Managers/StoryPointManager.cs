using System;
using Core.EventSystem;
using Main.GameStats;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardSystem;
using Main.StoryPoints;
using Main.StoryPoints.SPProviders;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Main.Managers {
    public class StoryPointManager : MonoBehaviour, IGameStateResponder {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        [SerializeField] private SEventManager gmEventManager;

        [Header("Visuals")] [SerializeField] private MStoryPoint storyPointPrefab;

        [FormerlySerializedAs("eventsPosition")] [SerializeField]
        private Transform storyPosition;


        private MStoryPoint _currentStory;
        private ISPProvider _spProvider;

        private void Awake() {
            _spProvider = GetComponent<ISPProvider>();
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
        }

        private void OnDestroy() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
        }

        private void Start() {
            NextStoryPoint();
        }

        public void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null) {
            if (state != GameState.StoryTurn) {
                return;
            }

            if (customArgs is OnBoardStateBroadcastEventArgs boardState) {
                StoryTurn(boardState.ElementsController);
            }

            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        private void StoryTurn(IBoardNeuronController elementsController) {
            // first SP
            if (_currentStory == null) {
                NextStoryPoint();
                return;
            }

            // in the middle of SP
            _currentStory.Decrement();
            // SP reached evaluation
            if (_currentStory.TurnsToEvaluation == 0) {
                _currentStory.Evaluate(elementsController);
                // current SP is done - next event
                NextStoryPoint();
            }
        }

        private void NextStoryPoint() {
            if (_spProvider.IsEmpty() && _currentStory.Evaluated) {
                Debug.Log("No more story points in queue!");
                storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
                return;
            }

            // var storyPointData = ReadStoryPointFromJson();
            var storyPointData = _spProvider.Next();

            var lastEvent = _currentStory;
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(storyPointData.description, storyPointData.reward, storyPointData.turnsToEvaluation,
                storyPointData.outcomes, storyPointData.statEffects);
            if (lastEvent != null) {
                Destroy(lastEvent.gameObject);
            }
        }

        #region EventHandlers

        public void OnAfterGameState(EventArgs eventData) {
            if (eventData is GameStateEventArgs stateData) {
                HandleAfterGameStateChanged(stateData.State, stateData.CustomArgs);
            }
        }


        #endregion
    }
}