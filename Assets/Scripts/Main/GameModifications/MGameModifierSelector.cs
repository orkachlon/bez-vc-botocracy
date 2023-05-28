using System;
using Core.EventSystem;
using UnityEngine;

namespace Main.GameModifications {
    public class MGameModifierSelector : MonoBehaviour {
        
        [Header("Modifiers"), SerializeField] private bool infiniteStoryPoints;

        [Header("Event Managers"), SerializeField]
        private SEventManager modificationEventManager;

        private void Start() {
            modificationEventManager.Raise(GameModificationEvents.OnInfiniteSP, new IsInfiniteStoryPointsEventArgs(infiniteStoryPoints));
        }
    }
}