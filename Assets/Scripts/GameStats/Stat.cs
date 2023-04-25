using System;
using UnityEngine;

namespace GameStats {
    public class Stat : MonoBehaviour, IStat {
        
        private float _value;
        public float Value {
            get => _value;
            set {
                _value = value;
                var scale = statFill.transform.localScale;
                statFill.transform.localScale = new Vector3(scale.x, Mathf.Clamp01(_value), scale.z);
            }
        }

        [SerializeField] private SpriteRenderer statFill;


        private void Awake() {
            Value = 1;
        }

        public bool IsInBounds(float lo, float hi) {
            return lo <= Value && Value <= hi;
        }
    }
}