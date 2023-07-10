using System;
using Core.EventSystem;
using DG.Tweening;
using Events.General;
using TMPro;
using Types.GameState;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Menus.EndGame.WinScreen {
    public class MWinScreenController : MOverlayScreen {
        
        [SerializeField] private SEventManager gmEventManager;
        
        [Header("Animation"), SerializeField] private RectTransform buttonContainer;
        [SerializeField] private RectTransform stats;
        [SerializeField] private RectTransform winTextRect;
        [SerializeField, Range(0, 0.5f)] private float singleCharAnimationDuration;
        
        [Header("Win Message"), SerializeField] private TextMeshProUGUI winMessageTextfield;
        [SerializeField, TextArea(5, 15)] private string winMessage;
        //[SerializeField] private AudioClip winMessageTypeSound;
        
        [Header("Stats"), SerializeField] private MStatDisplayer statDisplayer;
        
        private Tween _animation;
        private MStatCollector _statProvider;

        private AudioSource _as;

        #region UnityMethods

        protected override void Awake() {
            base.Awake();
            _statProvider = GetComponent<MStatCollector>();
            _as = GetComponent<AudioSource>();
        }

        private void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, ShowWinScreen);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, ShowWinScreen);
        }

        #endregion
        
        private void ShowWinScreen(EventArgs obj) {
            if (obj is not GameStateEventArgs { State: EGameState.Win }) {
                return;
            }
            _animation?.Kill();

            // set values to start of animation
            buttonContainer.anchoredPosition = new Vector2(-buttonContainer.sizeDelta.x, buttonContainer.anchoredPosition.y);
            stats.anchoredPosition = new Vector2(stats.sizeDelta.x, stats.anchoredPosition.y);
            winTextRect.anchoredPosition = new Vector2(winTextRect.anchoredPosition.x, -winTextRect.sizeDelta.y);
            
            // play animation
            _animation = ShowOverlay()
                // buttons slide in from left
                .Append(buttonContainer.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                // stats slide in from right
                .Join(stats.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                .Join(winTextRect.DOAnchorPosY(0, animationDuration).SetEase(animationEasing))
                .OnComplete(() => {
                    statDisplayer.AnimateStats(_statProvider, animationDuration);
                    AnimateWinMessage();
                });
        }

        public void AnimateWinMessage() {
            DOVirtual.Int(0, winMessage.Length, singleCharAnimationDuration * winMessage.Length, i => {
                winMessageTextfield.text = winMessage[..i];
                _as.pitch = 1 + (Random.value - 0.5f) * 0.1f;
                _as.Play();
            })
                .SetEase(Ease.Linear);
        }
    }
}