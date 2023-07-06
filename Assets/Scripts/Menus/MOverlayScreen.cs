using System;
using Core.EventSystem;
using DG.Tweening;
using Events.UI;
using PostProcessing;
using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    public abstract class MOverlayScreen : MonoBehaviour {
        [Header("Dependencies"), SerializeField] protected Blur bgBlurEffect;
        [SerializeField] private Image bgBlockerImg;

        [Header("Animation"), SerializeField] protected float animationDuration;
        [SerializeField] protected AnimationCurve animationEasing;
        
        [Header("Event Managers"), SerializeField] protected SEventManager uiEventManager;

        private Sequence _overlayAnimation;
        
        protected Color BGBlockerColor;

        protected virtual void Awake() {
            BGBlockerColor = bgBlockerImg.color;
        }

        protected virtual Sequence ShowOverlay() {
            uiEventManager.Raise(UIEvents.OnOverlayShow, EventArgs.Empty);
            _overlayAnimation?.Kill();
            bgBlockerImg.color -= new Color(0, 0, 0, bgBlockerImg.color.a);
            _overlayAnimation = DOTween.Sequence()
                .OnStart(() => {
                    bgBlockerImg.gameObject.SetActive(true);
                    bgBlurEffect.enabled = true;
                })
                // ramp up all blur values
                .Append(DOVirtual.Float(0, 10, animationDuration, f => bgBlurEffect.radius = f))
                .Join(DOVirtual.Int(1, 6, animationDuration, i => bgBlurEffect.qualityIterations = i))
                .Join(DOVirtual.Int(0, 3, animationDuration, i => bgBlurEffect.filter = i))
                // bring up blocker opacity
                .Join(bgBlockerImg.DOFade(BGBlockerColor.a, animationDuration));
            return _overlayAnimation;
        }

        protected virtual Sequence HideOverlay() {
            _overlayAnimation?.Kill();
            bgBlockerImg.color = BGBlockerColor;
            _overlayAnimation = DOTween.Sequence()
                // bring down all blur values
                .Append(DOVirtual.Float(0, 10, animationDuration, f => bgBlurEffect.radius = f).From())
                .Join(DOVirtual.Int(1, 6, animationDuration, i => bgBlurEffect.qualityIterations = i).From())
                .Join(DOVirtual.Int(0, 3, animationDuration, i => bgBlurEffect.filter = i).From())
                // bring down blocker opacity
                .Join(bgBlockerImg.DOFade(0, animationDuration))
                .OnComplete(() => {
                    bgBlockerImg.gameObject.SetActive(false);
                    bgBlurEffect.enabled = false;
                    uiEventManager.Raise(UIEvents.OnOverlayHide, EventArgs.Empty);
                });
            return _overlayAnimation;
        }
    }
}