using System;
using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Events.UI;
using TMPro;
using Types.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Message {
    public class MTutorialMessage : MonoBehaviour, IHideable, IShowable {

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image bg;
        [SerializeField] private int characterWrapLimit;

        [Header("Animation"), SerializeField] private float animationDuration; 
        [SerializeField] private AnimationCurve animationEasing;

        [Header("Event Managers"), SerializeField]
        private SEventManager uiEventManager;
        
        private LayoutElement layoutElement;

        #region UnityMethods

        private void Awake() {
            layoutElement = GetComponentInChildren<LayoutElement>();
        }

        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnGamePaused, Hide);
            uiEventManager.Register(UIEvents.OnGameUnpaused, Show);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, Hide);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, Show);
        }
        
        #endregion

        #region EventHandlers

        private async void Hide(EventArgs args) {
            await Hide();
        }
        
        private async void Show(EventArgs args) {
            await Show();
        }

        #endregion
        
        public void SetText(string message) {
            messageText.text = message;
            // set bg width with respect to message content, up to a maximum width.
            if (message.Contains("\n")) {
                var lines = message.Split("\n");
                var longestLine = lines.Max(l => l.Length);
                layoutElement.enabled = longestLine > characterWrapLimit;
            } else {
                layoutElement.enabled = message.Length > characterWrapLimit;
            }
        }

        public async Task AwaitShowAnimation() {
            bg.rectTransform.anchoredPosition = new Vector2(-bg.rectTransform.sizeDelta.x, bg.rectTransform.anchoredPosition.y);
            await Show();
        }

        public async Task AwaitHideAnimation() {
            await Hide();
        }

        public async Task Hide(bool immediate = false) {
            if (immediate) {
                bg.rectTransform.anchoredPosition =
                    new Vector2(-bg.rectTransform.sizeDelta.x, bg.rectTransform.anchoredPosition.y);
                return;
            }
            await bg.rectTransform.DOAnchorPosX(-bg.rectTransform.sizeDelta.x, animationDuration)
                .SetEase(animationEasing)
                .AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false) {
            if (immediate) {
                bg.rectTransform.anchoredPosition =
                    new Vector2(100, bg.rectTransform.anchoredPosition.y);
                return;
            }
            await bg.rectTransform.DOAnchorPosX(100, animationDuration)
                .SetEase(animationEasing)
                .AsyncWaitForCompletion();
        }
    }
}