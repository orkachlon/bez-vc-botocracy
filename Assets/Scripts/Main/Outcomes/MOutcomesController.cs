using System;
using System.Collections.Generic;
using Core.EventSystem;
using Main.StoryPoints;
using Main.StoryPoints.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Outcomes {
    public class MOutcomesController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [Header("Outcome Prefab"), SerializeField]
        private MUIOutcome outcomePrefab;

        [Header("Outcome Container"), SerializeField]
        private RectTransform verticalContainer;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private Queue<MUIOutcome> _outcomeQueue;

        private void Awake() {
            _outcomeQueue = new Queue<MUIOutcome>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }


        #region EventHandlers

        private void OnStoryEvaluated(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs || storyEventArgs.Story.DecisionEffects == TraitDecisionEffects.NoDecision) {
                return;
            }

            var storyEffects = storyEventArgs.Story.DecisionEffects;
            var newOutcome = Instantiate(outcomePrefab, verticalContainer);
            newOutcome.SetDecider(storyEffects.DecidingTrait.ToString());
            newOutcome.SetText(storyEffects.Outcome);
            _outcomeQueue.Enqueue(newOutcome);
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