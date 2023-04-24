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

        private void Start() {
            var eventText = eventData[0].text;
            if (string.IsNullOrEmpty(eventText)) {
                throw new JsonSerializationException("Event text was null or empty");
            }

            var data = JsonConvert.DeserializeObject<GameEventData>(eventText);
            var evnt = Instantiate(eventPrefab, eventsPosition.position, Quaternion.identity, transform);
            evnt.InitData(data.description, data.reward, data.statEffects);
        }
    }

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
}