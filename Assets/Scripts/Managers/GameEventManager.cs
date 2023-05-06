using System;
using System.Collections.Generic;
using GameEvents;
using GameStats;
using Traits;
using UnityEngine;
using Utils;
using Newtonsoft.Json;

namespace Managers {
    public class GameEventManager : MonoBehaviour, IGameStateResponder {

        [SerializeField] private GameEvent eventPrefab;
        [SerializeField] private Transform eventsPosition;
        
        [Header("Events")]
        [SerializeField] private List<TextAsset> eventTextAssets;

        public static GameEventManager Instance;
        public static event Action OnEventTurn;
        public static event Action OnNoMoreEvents;
        public static event Action OnEventEvaluated;

        private Queue<TextAsset> _eventQueue;
        private GameEvent _currentEvent;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            UnityEditor.AssetDatabase.Refresh();
            _eventQueue = new Queue<TextAsset>();
            EnqueueEvents(eventTextAssets);
            EventTurn();
            GameManager.OnAfterGameStateChanged += HandleAfterGameStateChanged;
        }

        private void OnDestroy() {
            GameManager.OnAfterGameStateChanged -= HandleAfterGameStateChanged;
        }
        
        public void EnqueueEvents(IEnumerable<TextAsset> eventsToAdd) {
            foreach (var newEvent in eventsToAdd) {
                _eventQueue.Enqueue(newEvent);
            }
        }

        public void HandleAfterGameStateChanged(GameManager.GameState state) {
            switch (state) {
                case GameManager.GameState.EventTurn:
                    EventTurn();
                    OnEventTurn?.Invoke();
                    break;
                case GameManager.GameState.EventEvaluation:
                    // EvaluateCurrentEvent();
                    break;
                case GameManager.GameState.Win:
                case GameManager.GameState.Lose:
                    // maybe display something
                    break;
                case GameManager.GameState.InitGrid:
                case GameManager.GameState.PlayerTurn:
                case GameManager.GameState.StatTurn:
                    // ignore these
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void EventTurn() {
            if (_eventQueue.Count == 0 && _currentEvent.Evaluated) {
                Debug.Log("No more events in queue!");
                OnNoMoreEvents?.Invoke();
                return;
            }

            // first event
            if (_currentEvent == null) {
                NextEvent();
                return;
            }
            // in the middle of event
            _currentEvent.Decrement();
            // if (!_currentEvent.Evaluated) {
            // }
            // event reached evaluation
            if (_currentEvent.TurnsToEvaluation == 0) {
                _currentEvent.Evaluate();
                // current event is done - next event
                NextEvent();
            }
        }

        public bool HasEvents() {
            return _eventQueue.Count > 0 || !_currentEvent.Evaluated;
        }

        private void EvaluateCurrentEvent() {
            if (_currentEvent.TurnsToEvaluation == 0) {
                _currentEvent.Evaluate();
                // OnEventEvaluated?.Invoke();
                return;
            }
            _currentEvent.Decrement();
        }

        private void NextEvent() {
            var eventData = ReadEventFromJson();

            var lastEvent = _currentEvent;
            _currentEvent = Instantiate(eventPrefab, eventsPosition.position, Quaternion.identity, transform);
            _currentEvent.InitData(eventData.description, eventData.reward, eventData.turnsToEvaluation, eventData.statEffects);
            if (lastEvent != null) {
                Destroy(lastEvent.gameObject);
            }
            // OnEventTurn?.Invoke();
        }

        private GameEventData ReadEventFromJson() {
            var eventText = _eventQueue.Dequeue().text;
            if (string.IsNullOrEmpty(eventText)) {
                throw new JsonSerializationException("Event text was null or empty");
            }
            var data = JsonConvert.DeserializeObject<GameEventData>(eventText);

            return data;
        }
    }

    #region Wrappers
    [Serializable]
    public struct GameEventData {
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