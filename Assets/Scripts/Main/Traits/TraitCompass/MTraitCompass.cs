using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Main.Traits.TraitCompass {
    public class MTraitCompass : MonoBehaviour {
        
        public RectTransform RectTransform { get; private set; }

        private void Awake() {
            RectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(RectTransform);
        }
    }
}