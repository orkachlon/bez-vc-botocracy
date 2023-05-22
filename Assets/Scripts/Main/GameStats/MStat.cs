using Core.EventSystem;
using UnityEngine;

namespace Main.GameStats {
    public class MStat : MonoBehaviour, IStat {
        [Header("Data Object"), SerializeField]
        private SStatData dataProvider;
        [SerializeField] private float initialValue = 0.5f;

        [Header("Event Managers"), SerializeField]
        private SEventManager statEventManager;
        
        [Header("Visuals"), SerializeField] private SpriteRenderer statFill;
        
        public float Value {
            get => DataProvider.Value;
            set {
                if (!IsInBounds(value)) {
                    statEventManager.Raise(StatEvents.OnStatOutOfBounds, new StatEventArgs(this));
                    // return; ???
                }

                DataProvider.Value = value;
                FitFillToValue();
                statEventManager.Raise(StatEvents.OnStatValueChanged, new StatEventArgs(this));
            }
        }

        public string Name => DataProvider.Name;

        private SStatData DataProvider => dataProvider;

        private void Awake() {
            DataProvider.Value = initialValue;
            FitFillToValue();
        }

        public bool IsInBounds() {
            return IsInBounds(Value);
        }

        private bool IsInBounds(float value) {
            return Value is > 0 and < 1;
        }

        private void FitFillToValue() {
            var scale = statFill.transform.localScale;
            statFill.transform.localScale = new Vector3(scale.x, Mathf.Clamp01(Value), scale.z);
        }
    }
}