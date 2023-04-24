using System;
using System.Collections.Generic;
using GameEvents;
using GameStats;
using Traits;
using UnityEngine;
using Utils;
using Newtonsoft.Json;

namespace Managers {
    public class GameEventManager : MonoBehaviour {

        [SerializeField] private GameEvent eventPrefab;
        [SerializeField] private Transform eventsPosition;
        
        [Header("Events")]
        [SerializeField] private List<TextAsset> eventData;

        public static GameEventManager Instance;
        public static event Action EventDone;
        public static event Action NoMoreEvents;

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
            GameManager.OnGameStateChanged += NextEvent;
        }

        private void OnDestroy() {
            GameManager.OnGameStateChanged -= NextEvent;
        }

        public void EnqueueEvents(IEnumerable<TextAsset> eventsToAdd) {
            foreach (var newEvent in eventsToAdd) {
                _eventQueue.Enqueue(newEvent);
            }
        }

        public void NextEvent(GameManager.GameState state) {
            if (!GameManager.GameState.EventTurn.Equals(state)) {
                return;
            }
            if (_eventQueue.Count == 0) {
                Debug.Log("No more events in queue!");
                NoMoreEvents?.Invoke();
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
            EventDone?.Invoke();
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