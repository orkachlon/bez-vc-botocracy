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
        [SerializeField] private List<TextAsset> eventData;

        public static GameEventManager Instance;
        public static event Action OnNextEvent;
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
            _eventQueue = new Queue<TextAsset>();
            EnqueueEvents(eventData);
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
                    NextEvent();
                    break;
                case GameManager.GameState.EventEvaluation:
                    EvaluateCurrentEvent();
                    break;
                case GameManager.GameState.Win:
                case GameManager.GameState.Lose:
                    // maybe display something
                    break;
                case GameManager.GameState.InitGrid:
                case GameManager.GameState.PlayerTurn:
                case GameManager.GameState.StatEvaluation:
                    // ignore these
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public bool HasEvents() {
            return _eventQueue.Count > 0;
        }

        private void EvaluateCurrentEvent() {
            _currentEvent.Evaluate();
            OnEventEvaluated?.Invoke();
        }

        private void NextEvent() {
            if (_eventQueue.Count == 0) {
                Debug.Log("No more events in queue!");
                OnNoMoreEvents?.Invoke();
                return;
            }
            var eventText = _eventQueue.Dequeue().text;
            if (string.IsNullOrEmpty(eventText)) {
                throw new JsonSerializationException("Event text was null or empty");
            }

            var data = JsonConvert.DeserializeObject<GameEventData>(eventText);
            if (_currentEvent != null) {
                _currentEvent.Hide(); 
                Destroy(_currentEvent);
            }
            _currentEvent = Instantiate(eventPrefab, eventsPosition.position, Quaternion.identity, transform);
            _currentEvent.InitData(data.description, data.reward, data.statEffects);
            OnNextEvent?.Invoke();
        }
    }

    #region Wrappers
    [Serializable]
    public struct GameEventData {
        public string description;
        public StatToTraitWeights statEffects;
        public int reward;
    }

    [Serializable]
    public class TraitWeights : SerializableDictionary<ETraitType, float> { }
    
    [Serializable]
    public class StatToTraitWeights : SerializableDictionary<EStatType, TraitWeights> { }
    #endregion
}