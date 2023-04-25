using System;
using GameStats;
using UnityEngine;

namespace Managers {
    public class StatManager : MonoBehaviour, IGameStateResponder {

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
            get => health.Value;
            private set => health.Value = value;
        }

        [SerializeField] private Stat economy;

        public float Economy {
            get => health.Value;
            private set => health.Value = value;
        }

        public static StatManager Instance;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
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

        private bool IsStatOutOfBounds() {
            return economy.IsInBounds(statLoLimit, statHiLimit) && defense.IsInBounds(statLoLimit, statHiLimit) &&
                   health.IsInBounds(statLoLimit, statHiLimit);
        }

        private void CheckForGameLossByStats() {
            if (IsStatOutOfBounds()) {
                OnStatsEvaluated?.Invoke(true);
                return;
            }
            OnStatsEvaluated?.Invoke(false);
        }

        public void HandleGameStateChanged(GameManager.GameState state) {
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
    }
}