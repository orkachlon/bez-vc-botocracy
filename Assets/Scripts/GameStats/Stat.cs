using System;
using UnityEngine;

namespace GameStats {
    public class Stat : MonoBehaviour, IStat {
        
        [SerializeField] private float value;
        public float Value {
            get => value;
            set {
                this.value = value;
                var scale = statFill.transform.localScale;
                statFill.transform.localScale = new Vector3(scale.x, Mathf.Clamp01(this.value), scale.z);
            }
        }

        [SerializeField] private SpriteRenderer statFill;


        private void Awake() {
            Value = value;
        }

        public bool IsInBounds(float lo, float hi) {
            return lo <= Value && Value <= hi;
        }
    }
}