using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Main.GameStats;
using UnityEngine;

namespace Main.Managers {
    public class StatManager : MonoBehaviour, IGameStateResponder, IEnumerable<MStat> {

        [Header("Event Managers"), SerializeField] private SEventManager statEventManager; 
        [SerializeField] private SEventManager gmEventManager;
        
        [SerializeField] private MStat welfare;

        public float Welfare {
            get => welfare.Value;
            private set => welfare.Value = value;
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

        private void Awake() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnAfterGameState);
            statEventManager.Register(StatEvents.OnPrintStats, OnPrintStats);
            statEventManager.Register(StatEvents.OnContributeToStat, OnContribute);
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
                case EStatType.Welfare:
                    Welfare += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public bool IsStatOutOfBounds() {
            return !economy.IsInBounds() || !defense.IsInBounds() ||
                   !welfare.IsInBounds();
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
            if (state == GameState.StatTurn) {
                CheckForGameLossByStats();
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

        private void OnContribute(EventArgs args) {
            if (args is StatContributeEventArgs statEventArgs) {
                Contribute(statEventArgs.StatType, statEventArgs.Amount);
            }
        }

        #endregion

        #region IEnumerable
        public IEnumerator<MStat> GetEnumerator() {
            return new List<MStat>() {welfare, defense, economy}.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}