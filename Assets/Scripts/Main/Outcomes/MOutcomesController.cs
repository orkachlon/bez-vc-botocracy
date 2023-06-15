using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using DG.Tweening;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using Main.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Outcomes {
    public class MOutcomesController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IExpandable {
        [Header("Outcome Prefab"), SerializeField]
        private MUIOutcome outcomePrefab;

        [Header("Outcome Containers"), SerializeField]
        private RectTransform verticalContainer;
        [SerializeField] private RectTransform scrollArea;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager uiEventManager;

        private Queue<MUIOutcome> _outcomeQueue;
        private RectTransform _rt;

        private void Awake() {
            _outcomeQueue = new Queue<MUIOutcome>();
            _rt = GetComponent<RectTransform>();
        }

        private void Start() {
            CollapseOnGameStart();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }


        public void Expand() {
            _rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, 2060), 0.5f);
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

        #endregion
    }
}