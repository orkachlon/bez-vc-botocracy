using System;
using Core.EventSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Main.GameStats {
    public class MStat : MonoBehaviour, IStat {
        [Header("Data Object"), SerializeField]
        private SStatData dataProvider;
        [SerializeField] private float initialValue = 0.5f;

        [Header("Event Managers"), SerializeField]
        private SEventManager statEventManager;

        [Header("Visuals"), SerializeField] private TextMeshPro title;
        [SerializeField] private SpriteRenderer statFill;
        
        public float Value {
            get => DataProvider.Value;
            set {
                if (!IsInBounds(value)) {
                    statEventManager.Raise(StatEvents.OnStatOutOfBounds, new StatEventArgs(this));
                    // return; ???
                }

                FitFillToValue(Math.Abs(DataProvider.Value - value) > 0.02f);
                DataProvider.Value = value;
                statEventManager.Raise(StatEvents.OnStatValueChanged, new StatEventArgs(this));
            }
        }

        public EStatType Name => DataProvider.Type;

        private SStatData DataProvider => dataProvider;

        private void Awake() {
            DataProvider.Value = initialValue;
            title.text = DataProvider.Type.ToString();
            FitFillToValue();
        }

        public bool IsInBounds() {
            return IsInBounds(Value);
        }

        private bool IsInBounds(float value) {
            return Value is > 0 and < 1;
        }

        private void FitFillToValue(bool valueWasChanged = false) {
            if (valueWasChanged) {
                DOVirtual.Color(statFill.color, Color.white, 0.2f, c => statFill.color = c).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutExpo); 
            }
            var scale = statFill.transform.localScale;
            statFill.transform.localScale = new Vector3(scale.x, Mathf.Clamp01(Value), scale.z);
        }
    }
}