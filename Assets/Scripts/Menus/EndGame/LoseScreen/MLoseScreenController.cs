using System;
using Core.EventSystem;
using DG.Tweening;
using Events.General;
using Types.GameState;
using UnityEngine;

namespace Menus.EndGame.LoseScreen {
    public class MLoseScreenController : MOverlayScreen {
        
        [SerializeField] private SEventManager gmEventManager;

        [Header("Lose Reason"), SerializeField]
        private MLoseMessagePresenter loseReasonPresenter;
        
        [Header("Animation"), SerializeField] private RectTransform buttonContainer;
        [SerializeField] private RectTransform stats;
        [SerializeField] private RectTransform loseMessage;
        
        [Header("Stats"), SerializeField] private MStatDisplayer statDisplayer;
        
        private Tween _animation;
        private MStatCollector _statProvider;

        #region UnityMethods

        protected override void Awake() {
            base.Awake();
            _statProvider = GetComponent<MStatCollector>();
        }

        private void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, ShowLoseScreen);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, ShowLoseScreen);
        }

        #endregion
        
        private void ShowLoseScreen(EventArgs obj) {
            if (obj is not GameStateEventArgs {State: EGameState.Lose, CustomArgs: LoseGameEventArgs loseArgs}) {
                return;
            }

            _animation?.Kill();

            // set values to start of animation
            buttonContainer.anchoredPosition = new Vector2(-buttonContainer.sizeDelta.x, buttonContainer.anchoredPosition.y);
            stats.anchoredPosition = new Vector2(stats.sizeDelta.x, stats.anchoredPosition.y);
            loseMessage.anchoredPosition = new Vector2(loseMessage.anchoredPosition.x, -loseMessage.sizeDelta.y);
            
            // play animation
            _animation = ShowOverlay()
                // buttons slide in from left
                .Append(buttonContainer.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                // stats slide in from right
                .Join(stats.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                .Join(loseMessage.DOAnchorPosY(0, animationDuration).SetEase(animationEasing))
                .OnComplete(() => {
                    statDisplayer.AnimateStats(_statProvider, animationDuration);
                    loseReasonPresenter.ShowMessage(loseArgs.LoseReason);
                });
        }

    }
}