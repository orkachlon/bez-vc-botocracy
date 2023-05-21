using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using GameStats;
using UnityEngine;

namespace Managers {
    public class StatManager : MonoBehaviour, IGameStateResponder, IEnumerable<MStat> {

        [Header("Event Managers"), SerializeField] private SEventManager statEventManager; 
        [SerializeField] private SEventManager gmEventManager;
        
        [SerializeField] private MStat health;

        public float Health {
            get => health.Value;
            private set => health.Value = value;
        }

        [SerializeField] private MStat defense;

        public float Defense {
            get => defense.Value;
            private set => defense.Value = value;
        }

        [SerializeField] private MStat economy;

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

            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            statEventManager.Register(StatEvents.OnPrintStats, OnPrintStats);
        }

        private void OnDestroy() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            statEventManager.Unregister(StatEvents.OnPrintStats, OnPrintStats);
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
            return !economy.IsInBounds() || !defense.IsInBounds() ||
                   !health.IsInBounds();
        }

        private void CheckForGameLossByStats() {
            if (IsStatOutOfBounds()) {
                // OnStatTurn?.Invoke(true);
                return;
            }
            // OnStatTurn?.Invoke(false);
            statEventManager.Raise(StatEvents.OnStatTurn, EventArgs.Empty);
        }

        public void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null) {
            switch (state) {
                case GameState.StatTurn:
                    CheckForGameLossByStats();
                    break;
                case GameState.InitGrid:
                case GameState.StoryTurn:
                case GameState.PlayerTurn:
                case GameState.EventEvaluation:
                    break;
                case GameState.Win:
                case GameState.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void PrintStats() {
            Debug.Log(string.Join("\n", this.Select(stat => $"{stat} -> {stat.Value}")));
        }

        #region EventHandlers

        private void OnAfterGameState(EventArgs eventArgs) {
            if (eventArgs is GameStateEventArgs gameStateEventArgs) {
                HandleAfterGameStateChanged(gameStateEventArgs.State);
            }
        }

        private void OnPrintStats(EventArgs eventArgs) {
            PrintStats();
        }

        #endregion

        #region IEnumerable
        public IEnumerator<MStat> GetEnumerator() {
            return new List<MStat>() {health, defense, economy}.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}