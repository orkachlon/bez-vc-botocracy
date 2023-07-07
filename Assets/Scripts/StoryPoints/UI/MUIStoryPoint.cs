using System;
using System.Threading.Tasks;
using Animation;
using Audio;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using Events.UI;
using TMPro;
using Types.Animation;
using Types.StoryPoint;
using Types.UI;
using UnityEngine;
using UnityEngine.UI;

namespace StoryPoints.UI {
    public class MUIStoryPoint : MonoBehaviour, IUIStoryPoint, IAnimatable, IHideable, IShowable {
        [Header("Visuals"), SerializeField] private Canvas spCanvas;
        [SerializeField] protected Image backGround; 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private GameObject turnSection;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private Image artwork;
        [SerializeField] private GameObject closeButton;
        
        [Header("Animation"), SerializeField] protected float decrementShakeStrength;
        [SerializeField] protected float animationDuration;
        [SerializeField] protected AnimationCurve animationEasing;
        
        [Header("Sound"), SerializeField] private AudioClip decrementSound;
        [SerializeField] private AudioClip evaluateSound;
  
        [Header("Decision"), SerializeField] private GameObject decisionSection;
        [SerializeField] private TextMeshProUGUI deciderText;
        [SerializeField] private TextMeshProUGUI decisionText;
        [SerializeField] private TextMeshProUGUI outcomeText;

        [Header("Event Managers"), SerializeField]
        protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager uiEventManager;

        private const string DeciderPrefix =
            "<size=75%><font=\"EzerBlockTRIALONLY-Regular SDF\">Decided by:\n</size></font>";
        private const string ActionPrefix = "<font=\"EzerBlock Bold SDF\">Action: </font>";
        private const string OutcomePrefix = "<font=\"EzerBlock Bold SDF\">Outcome: </font>";
        
        protected IStoryPoint SP;
        protected bool IsPopup = false;
        protected Tween CurrentAnimation;

        #region UnityMethods

        protected virtual  void Awake() {
            SP = GetComponent<IStoryPoint>();
        }

        protected virtual void OnEnable() {
            uiEventManager.Register(UIEvents.OnOverlayShow, PauseHide);
            uiEventManager.Register(UIEvents.OnOverlayHide, PauseShow);
        }

        protected virtual void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnOverlayShow, PauseHide);
            uiEventManager.Unregister(UIEvents.OnOverlayHide, PauseShow);
        }

        #endregion

        #region EventHandlers

        private void OnInitStory(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            InitSPUI();
        }

        private void OnDecrementStory(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            UpdateTurnCounter(storyEventArgs.Story.TurnsToEvaluation);
        }

        protected virtual async void PauseHide(EventArgs args) {
            await Hide();
        }
        
        protected virtual async void PauseShow(EventArgs args) {
            await Show();
        }

        #endregion

        public void InitSPUI() {
            title.text = SP.Title;
            description.text = SP.Description;
            artwork.sprite = SP.Artwork;
            UpdateTurnCounter(SP.TurnsToEvaluation);
        }

        public async Task PlayInitAnimation() {
            await Show();
        }

        public async Task PlayDecrementAnimation() {
            var baseCol = backGround.color;
            backGround.color = Color.white;
            StopCurrentAnimation();
            PlayDecrementSound();
            var seq = DOTween.Sequence()
                .Insert(0, backGround.DOColor(baseCol, 0.5f))
                .Insert(0, backGround.rectTransform.DOShakePosition(0.5f, Vector3.right * decrementShakeStrength, randomness: 0, fadeOut: true, randomnessMode:ShakeRandomnessMode.Harmonic))
                .OnComplete(() => CurrentAnimation = null);
            CurrentAnimation = seq;
            await AnimationManager.Register(this, seq.AsyncWaitForCompletion());
        }

        public async Task PlayEvaluateAnimation() {
            IsPopup = true;
            PlayEvaluateSound();
            await AnimationManager.WaitForElement(this);
            StopCurrentAnimation();
            CurrentAnimation = backGround.rectTransform.DOAnchorPosX(Screen.width * 0.5f / spCanvas.scaleFactor - backGround.rectTransform.sizeDelta.x * 0.5f, 0.2f)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    turnSection.SetActive(false);
                    closeButton.SetActive(true);
                    decisionSection.SetActive(true);
                    ShowDecisionData();
                    CurrentAnimation = null;
                });
            await CurrentAnimation.AsyncWaitForCompletion();
            
            StopCurrentAnimation();
            MCoroutineHelper.InvokeAfterNextFrame(() => {
                CurrentAnimation = backGround.rectTransform
                    .DOAnchorPosY(Screen.height * 0.5f / spCanvas.scaleFactor - backGround.rectTransform.sizeDelta.y * 0.5f, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => CurrentAnimation = null);
            });
        }

        public virtual async void CloseDecisionPopup() {
            StopCurrentAnimation();
            CurrentAnimation = backGround.rectTransform
                .DOAnchorPosY(-backGround.rectTransform.sizeDelta.y, animationDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => CurrentAnimation = null);
            await CurrentAnimation.AsyncWaitForCompletion();
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(SP));
        }

        public void UpdateTurnCounter(int turns) {
            turnCounter.text = $"{turns}";
        }

        private void ShowDecisionData() {
            deciderText.text = $"{DeciderPrefix}{SP.DecisionEffects.DecidingTrait}";
            decisionText.text = $"{ActionPrefix}{SP.DecisionEffects.Decision}";
            outcomeText.text = $"{OutcomePrefix}{SP.DecisionEffects.Outcome}";
        }

        public async Task Hide(bool immediate = false) {
            if (immediate) {
                if (IsPopup) {
                    backGround.rectTransform.anchoredPosition = new Vector2(backGround.rectTransform.anchoredPosition.x, -backGround.rectTransform.sizeDelta.y);
                    return;
                }
                backGround.rectTransform.anchoredPosition = new Vector2(-backGround.rectTransform.sizeDelta.x, backGround.rectTransform.anchoredPosition.y);
                return;
            }

            StopCurrentAnimation();
            CurrentAnimation = IsPopup ? 
                backGround.rectTransform.DOAnchorPosY(-backGround.rectTransform.sizeDelta.y, animationDuration).SetEase(animationEasing) : 
                backGround.rectTransform.DOAnchorPosX(-backGround.rectTransform.sizeDelta.x, animationDuration).SetEase(animationEasing);
            CurrentAnimation.OnComplete(() => CurrentAnimation = null);
            await CurrentAnimation.AsyncWaitForCompletion();
        }

        public async Task Show(bool immediate = false) {
            if (immediate) {
                if (IsPopup) {
                    var canvasScaleFactor = spCanvas.scaleFactor;
                    var bgSize = backGround.rectTransform.sizeDelta;
                    backGround.rectTransform.anchoredPosition = new Vector2(
                        Screen.width * 0.5f / canvasScaleFactor - bgSize.x * 0.5f,
                        Screen.height * 0.5f / canvasScaleFactor - bgSize.y * 0.5f);
                    return;
                }
                backGround.rectTransform.anchoredPosition = new Vector2(100, backGround.rectTransform.anchoredPosition.y);
                return;
            }

            StopCurrentAnimation();
            var screenHalfHeight = Screen.height * 0.5f / spCanvas.scaleFactor - backGround.rectTransform.sizeDelta.y * 0.5f;
            CurrentAnimation = IsPopup ? 
                backGround.rectTransform.DOAnchorPosY(screenHalfHeight, animationDuration).SetEase(animationEasing) :
                backGround.rectTransform.DOAnchorPosX(100, animationDuration).SetEase(animationEasing);
            CurrentAnimation.OnComplete(() => CurrentAnimation = null);
            await CurrentAnimation.AsyncWaitForCompletion();
        }

        private void StopCurrentAnimation() {
            CurrentAnimation?.Complete();
            CurrentAnimation?.Kill();
            CurrentAnimation = null;
        }

        private void PlayDecrementSound() {
            AudioSpawner.PoolSound(decrementSound);
        }

        private void PlayEvaluateSound() {
            AudioSpawner.PoolSound(evaluateSound);
        }
    }
}