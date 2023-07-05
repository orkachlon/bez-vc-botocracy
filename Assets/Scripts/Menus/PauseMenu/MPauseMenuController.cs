using System;
using Core.EventSystem;
using DG.Tweening;
using Events.UI;
using PostProcessing;
using UnityEngine;

namespace Menus.PauseMenu {
    public class MPauseMenuController : MonoBehaviour {

        [SerializeField] private Blur bgBlurEffect;
        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private MPauseBGController bgClickBlocker;
        [SerializeField, Range(0, 1)] private float bgBlockerOpacity;
        [SerializeField] private RectTransform controls;

        [SerializeField] private float animationDuration;
        [SerializeField] private AnimationCurve animationEasing;

        [SerializeField] private SEventManager uiEventManager;


        private Tween _animation;
        private bool _isPaused;
        
        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnGamePaused, ShowPauseScreen);
            uiEventManager.Register(UIEvents.OnGameUnpaused, HidePauseScreen);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnGamePaused, ShowPauseScreen);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, HidePauseScreen);
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.Escape)) {
                return;
            }

            uiEventManager.Raise(_isPaused ? UIEvents.OnGameUnpaused : UIEvents.OnGamePaused, new PauseArgs(!_isPaused));
        }

        private void ShowPauseScreen(EventArgs obj) {
            _isPaused = true;
            _animation?.Kill();
            _animation = DOTween.Sequence()
                .OnStart(() => {
                    bgClickBlocker.gameObject.SetActive(true);
                    bgBlurEffect.enabled = true;
                })
                // ramp up all blur values
                .Append(DOVirtual.Float(0, 10, animationDuration, f => bgBlurEffect.radius = f))
                .Join(DOVirtual.Int(1, 6, animationDuration, i => bgBlurEffect.qualityIterations = i))
                .Join(DOVirtual.Int(0, 3, animationDuration, i => bgBlurEffect.filter = i))
                // bring up blocker opacity
                .Join(DOVirtual.Color(new Color(1, 1, 1, 0), new Color(1, 1, 1, bgBlockerOpacity), animationDuration, c => bgClickBlocker.SetColor(c)))
                // buttons slide in
                .Join(buttonContainer.DOAnchorPosX(0, animationDuration).SetEase(animationEasing))
                // controls slide in
                .Join(controls.DOAnchorPosX(0, animationDuration).SetEase(animationEasing));
        }

        private void HidePauseScreen(EventArgs obj) {
            _isPaused = false;
            _animation?.Kill();
            _animation = DOTween.Sequence()
                // bring down all blur values
                .Append(DOVirtual.Float(0, 10, animationDuration, f => bgBlurEffect.radius = f).From())
                .Join(DOVirtual.Int(1, 6, animationDuration, i => bgBlurEffect.qualityIterations = i).From())
                .Join(DOVirtual.Int(0, 3, animationDuration, i => bgBlurEffect.filter = i).From())
                // bring down blocker opacity
                .Join(DOVirtual.Color(new Color(1, 1, 1, bgBlockerOpacity), new Color(1, 1, 1, 0), animationDuration, c => bgClickBlocker.SetColor(c)))
                // buttons slide out
                .Join(buttonContainer.DOAnchorPosX(-buttonContainer.sizeDelta.x, animationDuration).SetEase(animationEasing))
                // controls slide out
                .Join(controls.DOAnchorPosX(controls.sizeDelta.x, animationDuration).SetEase(animationEasing))
                .OnComplete(() => {
                    bgClickBlocker.gameObject.SetActive(false);
                    bgBlurEffect.enabled = false;
                });
        }
    }
}