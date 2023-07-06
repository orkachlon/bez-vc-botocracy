using System;
using Core.EventSystem;
using DG.Tweening;
using Events.General;
using Events.UI;
using Types.GameState;
using UnityEngine;

namespace Menus.PauseMenu {
    public class MPauseMenuController : MOverlayScreen {

        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private RectTransform controls;
        [SerializeField] private SEventManager gmEventManager;
        
        private Tween _animation;
        private bool _isPaused;
        private EGameState _gameState;

        #region UnityMethods

        private void OnEnable() {
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, UpdateGameState);
            uiEventManager.Register(UIEvents.OnGamePaused, ShowPauseScreen);
            uiEventManager.Register(UIEvents.OnGameUnpaused, HidePauseScreen);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, UpdateGameState);
            uiEventManager.Unregister(UIEvents.OnGamePaused, ShowPauseScreen);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, HidePauseScreen);
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.Escape) || _gameState is EGameState.Lose or EGameState.Win) {
                return;
            }

            uiEventManager.Raise(_isPaused ? UIEvents.OnGameUnpaused : UIEvents.OnGamePaused, new PauseArgs(!_isPaused));
        }

        #endregion

        private void UpdateGameState(EventArgs obj) {
            if (obj is not GameStateEventArgs stateArgs) {
                return;
            }
            _gameState = stateArgs.State;
        }

        private void ShowPauseScreen(EventArgs obj) {
            _isPaused = true;
            _animation?.Kill();
            buttonContainer.anchoredPosition =
                new Vector2(-buttonContainer.sizeDelta.x, buttonContainer.anchoredPosition.y);
            controls.anchoredPosition = new Vector2(controls.sizeDelta.x, controls.anchoredPosition.y);
            _animation = ShowOverlay()
                // buttons slide in
                .Append(buttonContainer.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                // controls slide in
                .Join(controls.DOAnchorPosX(0, animationDuration).SetEase(animationEasing));
        }

        private void HidePauseScreen(EventArgs obj) {
            _isPaused = false;
            _animation?.Kill();
            _animation = DOTween.Sequence()
                // buttons slide out
                .Append(buttonContainer.DOAnchorPosX(-buttonContainer.sizeDelta.x, animationDuration).SetEase(animationEasing))
                // controls slide out
                .Join(controls.DOAnchorPosX(controls.sizeDelta.x, animationDuration).SetEase(animationEasing))
                .Join(HideOverlay());
        }
    }
}