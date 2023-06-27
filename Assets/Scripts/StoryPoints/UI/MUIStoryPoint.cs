using System;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using TMPro;
using Types.Animation;
using Types.StoryPoint;
using UnityEngine;
using UnityEngine.UI;

namespace StoryPoints.UI {
    public class MUIStoryPoint : MonoBehaviour, IUIStoryPoint, IAnimatable {
        [Header("Visuals"), SerializeField] private Canvas spCanvas;
        [SerializeField] private Image backGround; 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private GameObject turnSection;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private Image artwork;
        [SerializeField] private GameObject closeButton;
        [SerializeField] private float decrementShakeStrength;
        
        [Header("Decision"), SerializeField] private GameObject decisionSection;
        [SerializeField] private TextMeshProUGUI deciderText;
        [SerializeField] private TextMeshProUGUI decisionText;
        [SerializeField] private TextMeshProUGUI outcomeText;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private const string DeciderPrefix =
            "<size=75%><font=\"EzerBlockTRIALONLY-Regular SDF\">Decided by:\n</size></font>";
        private const string ActionPrefix = "<font=\"EzerBlock Bold SDF\">Action: </font>";
        private const string OutcomePrefix = "<font=\"EzerBlock Bold SDF\">Outcome: </font>";
        
        private IStoryPoint _sp;

        #region UnityMethods

        private void Awake() {
            _sp = GetComponent<IStoryPoint>();
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

        #endregion

        public void InitSPUI() {
            title.text = _sp.Title;
            description.text = _sp.Description;
            artwork.sprite = _sp.Artwork;
            UpdateTurnCounter(_sp.TurnsToEvaluation);
        }

        public async Task PlayInitAnimation() {
            backGround.rectTransform.anchoredPosition = new Vector2(-backGround.rectTransform.sizeDelta.x, 50);
            await backGround.rectTransform.DOAnchorPosX(100, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        public async Task PlayDecrementAnimation() {
            var baseCol = backGround.color;
            backGround.color = Color.white;
            var seq = DOTween.Sequence()
                .Insert(0, backGround.DOColor(baseCol, 0.5f))
                .Insert(0, backGround.rectTransform.DOShakePosition(0.5f, Vector3.right * decrementShakeStrength, randomness: 0, fadeOut: true, randomnessMode:ShakeRandomnessMode.Harmonic))
                .AsyncWaitForCompletion();
            await AnimationManager.Register(this, seq);
        }

        public async Task PlayEvaluateAnimation() {
            await AnimationManager.WaitForElement(this);
            await backGround.rectTransform.DOAnchorPosX((Screen.width * 0.5f / spCanvas.scaleFactor) - (backGround.rectTransform.sizeDelta.x * 0.5f), 0.2f)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    turnSection.SetActive(false);
                    closeButton.SetActive(true);
                    decisionSection.SetActive(true);
                    ShowDecisionData();
                })
                .AsyncWaitForCompletion();
            MCoroutineHelper.InvokeAfterNextFrame(() => backGround.rectTransform
                .DOAnchorPosY(Screen.height * 0.5f / spCanvas.scaleFactor - backGround.rectTransform.sizeDelta.y * 0.5f, 0.5f)
                .SetEase(Ease.OutQuad));
        }

        public async void CloseDecisionPopup() {
            await backGround.rectTransform.DOAnchorPosY(-backGround.rectTransform.sizeDelta.y, 0.5f).SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(_sp));
        }

        public void UpdateTurnCounter(int turns) {
            turnCounter.text = $"{turns}";
        }

        private void ShowDecisionData() {
            deciderText.text = $"{DeciderPrefix}{_sp.DecisionEffects.DecidingTrait}";
            decisionText.text = $"{ActionPrefix}{_sp.DecisionEffects.Decision}";
            outcomeText.text = $"{OutcomePrefix}{_sp.DecisionEffects.Outcome}";
        }
    }
}