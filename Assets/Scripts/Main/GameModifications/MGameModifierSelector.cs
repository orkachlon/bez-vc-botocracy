﻿using System;
using Core.EventSystem;
using Events.General;
using UnityEngine;

namespace Main.GameModifications {
    public class MGameModifierSelector : MonoBehaviour {
        
        [Header("Modifiers"), SerializeField] private bool infiniteStoryPoints;
        [SerializeField] private bool infiniteNeurons;

        [Header("Event Managers"), SerializeField]
        private SEventManager modificationEventManager;

        private void Start() {
            modificationEventManager.Raise(GameModificationEvents.OnInfiniteSP, new IsInfiniteStoryPointsEventArgs(infiniteStoryPoints));
            modificationEventManager.Raise(GameModificationEvents.OnInfiniteNeurons, new IsInfiniteNeuronsEventArgs(infiniteNeurons));
        }
    }
}