using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.Board;
using Events.SP;
using Events.UI;
using TMPro;
using Types.Board;
using Types.StoryPoint;
using Types.Trait;
using Types.UI;
using UnityEngine;

namespace MyHexBoardSystem.UI {
    public class MTraitLabelPresenter : MonoBehaviour, IHideable, IShowable {
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager uiEventManager;

        [Header("Visuals"), SerializeField] protected ETrait trait;
        [SerializeField] protected TextMeshProUGUI textField;
        [SerializeField] private Color highlightColor;

        protected Color _baseColor;
        protected IBoardNeuronsController _neuronController;
        private DecidingTraits _currentDecidingTraits;

        protected virtual void Awake() {
            SetText(0);
            _baseColor = textField.color;
        }

        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
            storyEventManager.Register(StoryEvents.OnInitStory, SetIsDeciding);
            uiEventManager.Register(UIEvents.OnOverlayShow, PauseHide);
            uiEventManager.Register(UIEvents.OnOverlayHide, PauseShow);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateCounter);
            storyEventManager.Unregister(StoryEvents.OnInitStory, SetIsDeciding);
            uiEventManager.Unregister(UIEvents.OnOverlayShow, PauseHide);
            uiEventManager.Unregister(UIEvents.OnOverlayHide, PauseShow);
        }

        #region EventHandlers

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

        protected virtual async void PauseHide(EventArgs args) {
            await Hide();
        }
        
        protected virtual async void PauseShow(EventArgs args) {
            await Show();
        }

        #endregion

        protected void UpdateHighlightState() {
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

        protected virtual void Highlight() {
            textField.DOColor(highlightColor, 0.5f);
            // textField.color = highlightColor;
        }

        protected virtual void Lowlight() {
            textField.DOColor(_baseColor, 0.5f);
        }

        private bool IsCurrentlyDeciding() {
            return _currentDecidingTraits != null && _currentDecidingTraits.ContainsKey(trait);
        }

        private bool IsMaxTrait() {
            return _neuronController != null && _neuronController.GetMaxTrait(_currentDecidingTraits.Keys).Contains(trait);
        }

        public virtual async Task Hide(bool immediate = false) {
            if (immediate) {
                textField.color = new Color(textField.color.a, textField.color.g, textField.color.b, 0);
                return;
            }

            await textField.DOFade(0, 0.3f).AsyncWaitForCompletion();
        }

        public virtual async Task Show(bool immediate = false) {
            if (immediate) {
                textField.color = new Color(textField.color.a, textField.color.g, textField.color.b, 1);
                return;
            }

            await textField.DOFade(1, 0.3f).AsyncWaitForCompletion();
        }
    }
}