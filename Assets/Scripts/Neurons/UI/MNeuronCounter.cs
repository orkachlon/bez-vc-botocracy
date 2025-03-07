﻿using System;
using Core.EventSystem;
using Events.Board;
using TMPro;
using UnityEngine;

namespace Neurons.UI {
    public class MNeuronCounter : MonoBehaviour {
        
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private TextMeshProUGUI _counterText;

        #region UnityEvents

        private void Awake() {
            _counterText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateNeuronCount);
        }
        
        private void OnDisable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateNeuronCount);
        }

        #endregion
        
        private void UpdateNeuronCount(EventArgs obj) {
            if (obj is not BoardStateEventArgs boardArgs) {
                return;
            }

            _counterText.text = $"{boardArgs.ElementsController.CountNeurons}";

        }
    }
}