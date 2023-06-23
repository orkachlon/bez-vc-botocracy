using System;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using TMPro;
using Types.StoryPoint;
using UnityEngine;
using UnityEngine.UI;

namespace StoryPoints.UI {
    public class MUIStoryPoint : MonoBehaviour {
        [Header("Visuals"), SerializeField] private RectTransform backGround; 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private GameObject turnSection;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private Image artwork;
        [SerializeField] private GameObject closeButton;
        
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
        
        private Camera _camera;
        private IStoryPoint _sp;

        #region UnityMethods

        private void Awake() {
            _camera = Camera.main;
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
            backGround.anchoredPosition = new Vector2(-backGround.sizeDelta.x, 50);
            await backGround.DOAnchorPosX(50, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        public async Task PlayEvaluateAnimation() {
            var seq = DOTween.Sequence(this)
                .Append(backGround.DOAnchorPosX((_camera.pixelWidth * 0.5f) - (backGround.sizeDelta.x * 0.5f), 0.5f))
                .OnComplete(() => {
                    turnSection.SetActive(false);
                    closeButton.SetActive(true);
                    decisionSection.SetActive(true);
                    ShowDecisionData();
                });
            await seq.SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        public async void CloseDecisionPopup() {
            await backGround.DOAnchorPosY(-backGround.sizeDelta.y, 0.5f).SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
            storyEventManager.Raise(StoryEvents.OnEvaluate, new StoryEventArgs(_sp));
        }

        public void UpdateTurnCounter(int turns) {
            turnCounter.text = $"{turns}";
        }

        private void ShowDecisionData() {
            deciderText.text = $"{DeciderPrefix}{_sp.DecisionEffects.DecidingTrait.ToString()}";
            decisionText.text = $"{ActionPrefix}{_sp.DecisionEffects.Decision}";
            outcomeText.text = $"{OutcomePrefix}{_sp.DecisionEffects.Outcome}";
        }
    }
}