using System;
using Core.EventSystem;
using UnityEngine;

namespace GameStats {
    public class MStat : MonoBehaviour, IStat {
        [Header("Data Object"), SerializeField]
        private SStatData dataProvider;

        [Header("Event Managers"), SerializeField]
        private SEventManager statEventManager;
        
        [Header("Visuals"), SerializeField] private SpriteRenderer statFill;
        
        public float Value {
            get => DataProvider.Value;
            set {
                DataProvider.Value = value;
                if (!IsInBounds()) {
                    statEventManager.Raise(StatEvents.OnStatOutOfBounds, new StatEventArgs(this));
                    // return; ???
                }

                FitFillToValue();
                statEventManager.Raise(StatEvents.OnStatValueChanged, new StatEventArgs(this));
            }
        }

        public string Name => DataProvider.Name;

        private SStatData DataProvider => dataProvider;

        private void Awake() {
            FitFillToValue();
        }

        public bool IsInBounds() {
            return Value is <= 0 and <= 1;
        }

        private void FitFillToValue() {
            var scale = statFill.transform.localScale;
            statFill.transform.localScale = new Vector3(scale.x, Mathf.Clamp01(Value), scale.z);
        }
    }
}