using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStats;
using UnityEngine;

namespace Managers {
    public class StatManager : MonoBehaviour, IGameStateResponder, IEnumerable<Stat> {

        [SerializeField] private float statLoLimit = 0;
        [SerializeField] private float statHiLimit = 1;

        public static event Action<bool> OnStatsEvaluated;
        
        [SerializeField] private Stat health;

        public float Health {
            get => health.Value;
            private set => health.Value = value;
        }

        [SerializeField] private Stat defense;

        public float Defense {
            get => defense.Value;
            private set => defense.Value = value;
        }

        [SerializeField] private Stat economy;

        public float Economy {
            get => economy.Value;
            private set => economy.Value = value;
        }

        public static StatManager Instance;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }

            GameManager.OnAfterGameStateChanged += HandleAfterGameStateChanged;
        }

        private void OnDestroy() {
            GameManager.OnAfterGameStateChanged -= HandleAfterGameStateChanged;

        }

        public void Contribute(EStatType stat, float amount) {
            switch (stat) {
                case EStatType.Economy:
                    Economy += amount;
                    break;
                case EStatType.Defense:
                    Defense += amount;
                    break;
                case EStatType.Health:
                    Health += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public bool IsStatOutOfBounds() {
            return !economy.IsInBounds(statLoLimit, statHiLimit) || !defense.IsInBounds(statLoLimit, statHiLimit) ||
                   !health.IsInBounds(statLoLimit, statHiLimit);
        }

        private void CheckForGameLossByStats() {
            if (IsStatOutOfBounds()) {
                OnStatsEvaluated?.Invoke(true);
                return;
            }
            OnStatsEvaluated?.Invoke(false);
        }

        public void HandleAfterGameStateChanged(GameManager.GameState state) {
            switch (state) {
                case GameManager.GameState.StatEvaluation:
                    CheckForGameLossByStats();
                    break;
                case GameManager.GameState.InitGrid:
                case GameManager.GameState.EventTurn:
                case GameManager.GameState.PlayerTurn:
                case GameManager.GameState.EventEvaluation:
                    break;
                case GameManager.GameState.Win:
                case GameManager.GameState.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void PrintStats() {
            Debug.Log(string.Join("\n", Instance.Select(stat => $"{stat} -> {stat.Value}")));
        }

        #region IEnumerable
        public IEnumerator<Stat> GetEnumerator() {
            return new List<Stat>() {health, defense, economy}.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}