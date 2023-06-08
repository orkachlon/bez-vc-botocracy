using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.Events;
using TMPro;
using UnityEngine;

namespace Main.Neurons.UI {
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
            if (obj is not OnBoardStateBroadcastEventArgs boardArgs) {
                return;
            }

            _counterText.text = $"{boardArgs.ElementsController.CountNeurons}";

        }
    }
}