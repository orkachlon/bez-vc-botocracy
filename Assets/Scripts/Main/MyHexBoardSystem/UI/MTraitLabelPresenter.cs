using System;
using Core.EventSystem;
using Main.MyHexBoardSystem.Events;
using Main.Traits;
using TMPro;
using UnityEngine;

namespace Main.MyHexBoardSystem.UI {
    public class MTraitLabelPresenter : MonoBehaviour {
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        [Header("Visuals"), SerializeField] private ETrait trait;
        [SerializeField] private TextMeshProUGUI textField;

        private void Awake() {
            SetText(0);
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
        }

        private void UpdateCounter(EventArgs eventArgs) {
            if (eventArgs is not OnBoardStateBroadcastEventArgs args) {
                return;
            }
            SetText(args.ElementsController.GetTraitCount(trait));
        }

        private void SetText(int amount) {
            textField.text = $"{amount}\n{trait}";
        }
    }
}