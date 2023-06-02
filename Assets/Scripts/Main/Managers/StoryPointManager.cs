using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.Events;
using Main.StoryPoints;
using Main.StoryPoints.SPProviders;
using UnityEngine;

namespace Main.Managers {
    public class StoryPointManager : MonoBehaviour, IGameStateResponder {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager gmEventManager;
        [SerializeField] private SEventManager boardEventManager;

        [Header("Visuals")] [SerializeField] private MStoryPoint storyPointPrefab;
        
        private IStoryPoint _currentStory;
        private ISPProvider _spProvider;

        #region UnityMethods

        private void Awake() {
            _spProvider = GetComponent<ISPProvider>();
        }

        private void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #endregion

        public void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null) {
            if (state != GameState.StoryTurn) {
                return;
            }

            if (customArgs is OnBoardStateBroadcastEventArgs boardState) {
                StoryTurn(boardState.ElementsController);
            }

            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        private void Init(EventArgs obj) {
            NextStoryPoint();
        }

        private void StoryTurn(IBoardNeuronsController elementsController) {
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

            _currentStory?.Destroy();
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(storyPointData);
        }

        #region EventHandlers

        private void OnAfterGameState(EventArgs eventData) {
            if (eventData is GameStateEventArgs stateData) {
                HandleAfterGameStateChanged(stateData.State, stateData.CustomArgs);
            }
        }


        #endregion
    }
}