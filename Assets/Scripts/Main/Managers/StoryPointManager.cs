using System;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.Events;
using Main.GameStats;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.StoryPoints;
using Main.Traits;
using Main.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Main.Managers {
    public class StoryPointManager : MonoBehaviour, IGameStateResponder {

        [Header("Event Managers"), SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager gmEventManager;
        
        [Header("Visuals")]
        [SerializeField] private StoryPoint storyPointPrefab;
        [FormerlySerializedAs("eventsPosition")] [SerializeField] private Transform storyPosition;
        
        [Header("Story Points")]
        [SerializeField] private List<TextAsset> storyTextAssets;

        public static StoryPointManager Instance;

        private Queue<TextAsset> _eventQueue;
        private StoryPoint _currentStory;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            UnityEditor.AssetDatabase.Refresh();
            _eventQueue = new Queue<TextAsset>();
            EnqueueStoryPoints(storyTextAssets);
            NextStoryPoint();
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            // GameManager.OnAfterGameStateChanged += HandleAfterGameStateChanged;
        }

        private void OnDestroy() {
            // GameManager.OnAfterGameStateChanged -= HandleAfterGameStateChanged;
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
        }
        
        public void EnqueueStoryPoints(IEnumerable<TextAsset> eventsToAdd) {
            foreach (var newEvent in eventsToAdd) {
                _eventQueue.Enqueue(newEvent);
            }
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
            // first event
            if (_currentStory == null) {
                NextStoryPoint();
                return;
            }
            // in the middle of event
            _currentStory.Decrement();
            // if (!_currentStory.Evaluated) {
            // }
            // event reached evaluation
            if (_currentStory.TurnsToEvaluation == 0) {
                _currentStory.Evaluate(elementsController);
                // current event is done - next event
                NextStoryPoint();
            }
        }

        public bool HasEvents() {
            return _eventQueue.Count > 0 || !_currentStory.Evaluated;
        }

        private void NextStoryPoint() {
            if (_eventQueue.Count == 0 && _currentStory.Evaluated) {
                Debug.Log("No more story points in queue!");
                storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
                return;
            }
            var eventData = ReadEventFromJson();

            var lastEvent = _currentStory;
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(eventData.description, eventData.reward, eventData.turnsToEvaluation, eventData.statEffects);
            if (lastEvent != null) {
                Destroy(lastEvent.gameObject);
            }
        }

        private StoryPointData ReadEventFromJson() {
            var eventText = _eventQueue.Dequeue().text;
            if (string.IsNullOrEmpty(eventText)) {
                throw new JsonSerializationException("Event text was null or empty");
            }
            var data = JsonConvert.DeserializeObject<StoryPointData>(eventText);

            return data;
        }

        #region EventHandlers

        public void OnAfterGameState(EventArgs eventData) {
            if (eventData is GameStateEventArgs stateData) {
                HandleAfterGameStateChanged(stateData.State, stateData.CustomArgs);
            }
        }
        

        #endregion
    }

    #region Wrappers
    [Serializable]
    public struct StoryPointData {
        public string description;
        public StatToTraitWeights statEffects;
        public int reward;
        public int turnsToEvaluation;
    }

    [Serializable]
    public class TraitWeights : SerializableDictionary<ETraitType, float> { }
    
    [Serializable]
    public class StatToTraitWeights : SerializableDictionary<EStatType, TraitWeights> { }
    #endregion
}