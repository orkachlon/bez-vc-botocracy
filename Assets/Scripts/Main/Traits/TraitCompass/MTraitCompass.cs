using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils.DataStructures;
using Main.MyHexBoardSystem.Events;
using UnityEngine;
using UnityEngine.Assertions;

namespace Main.Traits.TraitCompass {
    public class MTraitCompass : MonoBehaviour {
        [SerializeField] private CompassDirectionDict compassDirections;
        [SerializeField] private float mouseDistanceTillDisabled;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        public RectTransform RectTransform { get; private set; }
        
        private void Awake() {
            RectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(RectTransform);
        }

        private void OnDisable() {
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassHide, new TraitCompassHoverEventArgs(null));
        }

        private void Update() {
            if (Vector2.Distance(Input.mousePosition, RectTransform.position) > mouseDistanceTillDisabled) {
                gameObject.SetActive(false);
            }
        }

        public void SetDecidingTraits(IEnumerable<ETrait> decidingTraits) {
            // reset all traits to false
            foreach (var trait in compassDirections.Keys) {
                compassDirections[trait].HasEffect = false;
            }
            
            // mark deciding traits
            foreach (var trait in decidingTraits) {
                if (!compassDirections.ContainsKey(trait)) {
                    continue;
                }
                compassDirections[trait].HasEffect = true;
            }
        }
    }
    
    [Serializable]
    internal class CompassDirectionDict : UDictionary<ETrait, MTraitCompassDirection> { }
}