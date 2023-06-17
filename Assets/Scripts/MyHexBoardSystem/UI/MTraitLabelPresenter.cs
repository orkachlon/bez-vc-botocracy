using System;
using System.Linq;
using Core.EventSystem;
using Events.SP;
using MyHexBoardSystem.Events;
using TMPro;
using Types.Board;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;

namespace MyHexBoardSystem.UI {
    public class MTraitLabelPresenter : MonoBehaviour {
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;

        [Header("Visuals"), SerializeField] private ETrait trait;
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Color highlightColor;

        private Color _baseColor;
        private IBoardNeuronsController _neuronController;
        private DecidingTraits _currentDecidingTraits;

        private void Awake() {
            SetText(0);
            _baseColor = textField.color;
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
            storyEventManager.Register(StoryEvents.OnInitStory, SetIsDeciding);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
            storyEventManager.Unregister(StoryEvents.OnInitStory, SetIsDeciding);
        }

        private void SetIsDeciding(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentDecidingTraits = storyEventArgs.Story.DecidingTraits;
            UpdateHighlightState();
        }

        private void UpdateCounter(EventArgs eventArgs) {
            if (eventArgs is not BoardStateEventArgs args) {
                return;
            }

            _neuronController = args.ElementsController;
            SetText(_neuronController.GetTraitCount(trait));
            UpdateHighlightState();
        }

        private void UpdateHighlightState() {
            if (IsCurrentlyDeciding() && IsMaxTrait()) {
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

        private bool IsCurrentlyDeciding() {
            return _currentDecidingTraits != null && _currentDecidingTraits.ContainsKey(trait);
        }

        private bool IsMaxTrait() {
            return _neuronController != null && _neuronController.GetMaxTrait(_currentDecidingTraits.Keys).Contains(trait);
        }
    }
}