using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.EventSystem;
using DG.Tweening;
using Events.SP;
using Events.UI;
using Types.StoryPoint;
using Types.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StoryPoints.Outcomes {
    public class MOutcomesController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IExpandable, IHideable, IShowable {
        [Header("Outcome Prefab"), SerializeField]
        private MUIOutcome outcomePrefab;

        [Header("Outcome Containers"), SerializeField] protected Canvas outcomePanelCanvas;
        [SerializeField] protected RectTransform verticalContainer;
        [SerializeField] protected RectTransform scrollArea;

        [Header("Animation"), SerializeField] protected float animationDuration;
        [SerializeField] protected AnimationCurve animationEasing;

        [Header("Event Managers"), SerializeField]
        protected SEventManager storyEventManager;
        [SerializeField] private SEventManager uiEventManager;

        private Queue<MUIOutcome> _outcomeQueue;
        private RectTransform _rt;
        protected Image BG;
        protected int Count => _outcomeQueue.Count;

        protected virtual void Awake() {
            _outcomeQueue = new Queue<MUIOutcome>();
            _rt = GetComponent<RectTransform>();
            BG = GetComponent<Image>();
        }

        protected virtual void Start() {
            CollapseOnGameStart();
        }

        protected virtual void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnStoryEvaluated);
            uiEventManager.Register(UIEvents.OnGamePaused, PauseHide);
            uiEventManager.Register(UIEvents.OnGameUnpaused, PauseShow);
        }

        protected virtual void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnStoryEvaluated);
            uiEventManager.Unregister(UIEvents.OnGamePaused, PauseHide);
            uiEventManager.Unregister(UIEvents.OnGameUnpaused, PauseShow);
        }


        public void Expand() {
            _rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, (Screen.height / outcomePanelCanvas.scaleFactor) - 100), 0.5f);
            foreach (var uiOutcome in _outcomeQueue) {
                uiOutcome.gameObject.SetActive(true);
            }
        }

        public void Collapse() {
            var sizeDelta = _rt.sizeDelta;
            if (_outcomeQueue.Count == 0) {
                sizeDelta = new Vector2(sizeDelta.x,
                    outcomePrefab.GetComponent<RectTransform>().rect.height - scrollArea.sizeDelta.y);
            } else {
                var latestOutcome = _outcomeQueue.ToArray()[^1];
                latestOutcome.gameObject.SetActive(true);
                
                sizeDelta = new Vector2(sizeDelta.x,
                    latestOutcome.GetComponent<RectTransform>().rect.height - scrollArea.sizeDelta.y);
            }
            // scale
            _rt.DOSizeDelta(sizeDelta, 0.5f);

            // disable all but most recent outcome
            var outcomesArray = _outcomeQueue.ToArray();
            for (var i = 0; i< _outcomeQueue.Count - 1; i++) {
                outcomesArray[i].gameObject.SetActive(false);
            }
        }

        private void CollapseOnGameStart() {
            _rt.sizeDelta = new Vector2(_rt.sizeDelta.x,
                outcomePrefab.GetComponent<RectTransform>().rect.height - scrollArea.sizeDelta.y);
        }

        private IEnumerator UpdateUI() {
            yield return null;
            uiEventManager.Raise(UIEvents.OnOutcomeAdded, EventArgs.Empty);
        }

        #region EventHandlers

        private void OnStoryEvaluated(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs || storyEventArgs.Story.DecisionEffects == TraitDecisionEffects.NoDecision) {
                return;
            }

            var storyEffects = storyEventArgs.Story.DecisionEffects;
            var newOutcome = Instantiate(outcomePrefab, verticalContainer);
            newOutcome.SetSPTitle(storyEventArgs.Story.Title);
            newOutcome.SetDecider(storyEffects.DecidingTrait.ToString());
            newOutcome.SetDecision(storyEffects.Decision);
            newOutcome.SetOutcomeText(storyEffects.Outcome);
            newOutcome.SetArtwork(storyEventArgs.Story.Artwork);
            _outcomeQueue.Enqueue(newOutcome);
            StartCoroutine(UpdateUI());
        }

        public void OnPointerEnter(PointerEventData eventData) {
            storyEventManager.Raise(StoryEvents.OnOutcomesEnter, EventArgs.Empty);
        }

        public void OnPointerExit(PointerEventData eventData) {
            storyEventManager.Raise(StoryEvents.OnOutcomesExit, EventArgs.Empty);
        }

        protected virtual async void PauseHide(EventArgs args) {
            await Hide();
        }
        
        protected virtual async void PauseShow(EventArgs args) {
            await Show();
        }

        #endregion

        public virtual async Task Hide(bool immediate = false) {
            if (immediate) {
                BG.rectTransform.anchoredPosition =
                    new Vector2(BG.rectTransform.sizeDelta.x, BG.rectTransform.anchoredPosition.y);
                return;
            }

            await BG.rectTransform.DOAnchorPosX(BG.rectTransform.sizeDelta.x, animationDuration)
                .SetEase(animationEasing)
                .AsyncWaitForCompletion();
        }

        public virtual async Task Show(bool immediate = false) {
            if (immediate) {
                BG.rectTransform.anchoredPosition =
                    new Vector2(0, BG.rectTransform.anchoredPosition.y);
                return;
            }

            await BG.rectTransform.DOAnchorPosX(-100, animationDuration)
                .SetEase(animationEasing)
                .AsyncWaitForCompletion();
        }
    }
}