using Managers;
using TMPro;
using UnityEngine;

namespace GameEvents {
    public class GameEvent : MonoBehaviour, IGameEvent {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TextMeshPro description;

        private int _reward;
        private StatToTraitWeights _calculationDict;

        public void InitData(string eventDescription, int reward, StatToTraitWeights calculationDict) {
            description.text = eventDescription;
            _reward = reward;
            _calculationDict = calculationDict;
        }
    }
}