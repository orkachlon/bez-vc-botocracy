using System;
using System.Collections.Generic;
using Core.EventSystem;
using Main.StoryPoints;
using Main.StoryPoints.SPProviders;
using UnityEngine;

namespace Main.Outcomes {
    public class MOutcomesController : MonoBehaviour {
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
            newOutcome.SetText($"{storyEffects.DecidingTrait}: {storyEffects.Outcome}");
            _outcomeQueue.Enqueue(newOutcome);
        }

        #endregion
    }
}