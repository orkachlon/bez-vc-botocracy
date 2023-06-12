using System;
using Core.EventSystem;
using DG.Tweening;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Outcomes {
    public class MDecisionPopup : MonoBehaviour {

        [Header("Visuals"), SerializeField] private TextMeshProUGUI storyPointTitle; 
        [SerializeField] private TextMeshProUGUI deciderText;
        [SerializeField] private TextMeshProUGUI decisionText;
        [SerializeField] private TextMeshProUGUI outcomeText;
        [SerializeField] private Image spImage;
        [SerializeField] private Image bg;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;


        private RectTransform _rt;
        private bool _isShowing = false;
        private Tween _currentAnimation;
        private IStoryPoint _currentSP;

        private const int StartXAnchoredPos = -50;

        private void Awake() {
            _rt = bg.GetComponent<RectTransform>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, SpawnPopup);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, SpawnPopup);
        }

        private void SpawnPopup(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }

            _currentSP = storyEventArgs.Story;

            if (_isShowing) {
                Hide();
            }
            Show();
        }

        public async void Hide() {
            if (_currentAnimation != null && _currentAnimation.IsPlaying()) {
                await _currentAnimation.AsyncWaitForCompletion();
            }
            _isShowing = false;
            _currentAnimation = _rt.DOAnchorPosX(_rt.rect.width, 0.5f);
        }

        private async void Show() {
            if (_currentAnimation != null && _currentAnimation.IsPlaying()) {
                await _currentAnimation.AsyncWaitForCompletion();
            }
            storyPointTitle.text = _currentSP.Title;
            deciderText.text = _currentSP.DecisionEffects.DecidingTrait.ToString();
            decisionText.text = _currentSP.DecisionEffects.Decision;
            outcomeText.text = _currentSP.DecisionEffects.Outcome;
            spImage.sprite = _currentSP.Artwork;
            _isShowing = true;
            _currentAnimation = _rt.DOAnchorPosX(StartXAnchoredPos, 0.5f).Play();
        }
    }
}