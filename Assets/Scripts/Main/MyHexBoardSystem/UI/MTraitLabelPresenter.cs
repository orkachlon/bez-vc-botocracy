using System;
using System.Linq;
using Core.EventSystem;
using DG.DemiEditor;
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
        [SerializeField] private Color highlightColor;

        private Color _baseColor;

        private void Awake() {
            SetText(0);
            _baseColor = textField.color;
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
            if (args.ElementsController.GetMaxTrait().Contains(trait)) {
                Highlight();
            }
            else {
                Lowlight();
            }
        }

        private void SetText(int amount) {
            textField.text = $"{amount}\n{trait}";
        }

        private void Highlight() {
            textField.color = highlightColor;
        }

        private void Lowlight() {
            textField.color = _baseColor;
        }
    }
}