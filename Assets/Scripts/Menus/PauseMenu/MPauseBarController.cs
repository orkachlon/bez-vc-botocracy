using System;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using DG.Tweening;
using Events.UI;
using Types.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.PauseMenu {
    public class MPauseBarController : MonoBehaviour, IHideable, IShowable {

        [SerializeField] private float animationDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager uiEventManager;

        private RectTransform _rt;
        private Image _bg;

        private void Awake() {
            _bg = GetComponent<Image>();
            _rt = _bg.rectTransform;
        }

        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnGamePaused, Hide);
            uiEventManager.Register(UIEvents.OnGameUnpaused, Show);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, Hide);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, Show);
        }

        private async void Hide(EventArgs args) {
            await Hide();
        }
        
        private async void Show(EventArgs args) {
            await Show();
        }

        public async Task Hide(bool immediate = false) {
            if (immediate) {
                _rt.anchoredPosition = new Vector2(_rt.anchoredPosition.x, _rt.sizeDelta.y);
                return;
            }

            await DOTween.Sequence()
                // .Append(_rt.DOAnchorPosY(_rt.sizeDelta.y, animationDuration))
                .Join(_bg.DOColor(AnimationConstants.UIPurple, animationDuration))
                .AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false) {
            if (immediate) {
                _rt.anchoredPosition = new Vector2(_rt.anchoredPosition.x, -100);
                return;
            }

            await DOTween.Sequence()
                // .Append(_rt.DOAnchorPosY(0, animationDuration))
                .Join(_bg.DOColor(AnimationConstants.UIBlue, animationDuration))
                .AsyncWaitForCompletion();
        }
    }
}