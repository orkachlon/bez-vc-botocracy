using System;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.Tutorial;
using MyHexBoardSystem.BoardSystem;
using MyHexBoardSystem.UI;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.Traits.Labels {
    public class MTutorialTraitLabelController : MTraitLabelPresenter {

        [Header("Dependencies"), SerializeField] private MTraitAccessor traitAccessor;
        [Header("Animation"), SerializeField] private int distanceFromBoard;
        [SerializeField] private int outOfScreenDistance;
        [SerializeField] private float animationDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager tutorialEventManager;

        public bool IsSPEnabled { get; set; }
        public bool IsPauseEnabled { get; set; }

        #region UnityMethods

        protected override async void Awake() {
            base.Awake();
            await Hide(true);
        }

        protected override void OnEnable() {
            base.OnEnable();
            tutorialEventManager.Register(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }

        protected override void OnDisable() {
            base.OnDisable();
            tutorialEventManager.Unregister(TutorialEvents.OnBeforeStage, UpdatePauseState);
        }

        #endregion

        #region EventHandlers

        private void UpdatePauseState(EventArgs obj) {
            if (obj is not TutorialStageEventArgs tutArgs) {
                return;
            }

            IsPauseEnabled = tutArgs.Stage switch {
                ETutorialStage.Introduction => false,
                ETutorialStage.NeuronRewards => false,
                ETutorialStage.Personalities => true,
                ETutorialStage.BoardEffects => true,
                ETutorialStage.Decisions => true,
                ETutorialStage.NeuronTypeIntro => true,
                ETutorialStage.ExpanderType => true,
                ETutorialStage.TravellerType => true,
                ETutorialStage.TimerType => true,
                ETutorialStage.CullerType => true,
                ETutorialStage.End => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void PauseHide(EventArgs args) {
            if (IsPauseEnabled) {
                base.Hide();
            }
        }

        protected override void PauseShow(EventArgs args) {
            if (IsPauseEnabled) {
                base.Show();
            }
        }

        #endregion

        public override async Task Hide(bool immediate = false) {
            var direction = traitAccessor.TraitToVectorDirection(trait).normalized;
            if (immediate) {
                textField.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0);
                textField.rectTransform.anchoredPosition = direction * outOfScreenDistance;
                textField.enabled = false;
                return;
            }
            var rt = textField.rectTransform;
            rt.anchoredPosition = direction * distanceFromBoard;
            await Task.WhenAll(
                rt.DOAnchorPos(direction * outOfScreenDistance, animationDuration).AsyncWaitForCompletion(),
                textField.DOFade(0, animationDuration).AsyncWaitForCompletion());
            textField.enabled = false;
        }

        public override async Task Show(bool immediate = false) {
            textField.enabled = true;
            textField.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 1);
            if (immediate) {
                textField.rectTransform.anchoredPosition = traitAccessor.TraitToVectorDirection(trait).normalized * distanceFromBoard;
                return;
            }
            var direction = traitAccessor.TraitToVectorDirection(trait).normalized;
            var rt = textField.rectTransform;
            rt.anchoredPosition = direction * outOfScreenDistance;
            await Task.WhenAll(
                rt.DOAnchorPos(direction * distanceFromBoard, animationDuration).AsyncWaitForCompletion(),
                textField.DOFade(1, animationDuration).AsyncWaitForCompletion());
        }

        public void UpdateColor() {
            UpdateHighlightState();
        }

        protected override void Highlight() {
            if (IsSPEnabled) {
                base.Highlight();
            } else {
                Lowlight();
            }
        }

        protected override void Lowlight() {
            if (IsSPEnabled) {
                textField.DOColor(Color.white, 0.5f);
            } else {
                textField.DOColor(_baseColor, 0.5f);
            }
        }
    }
}