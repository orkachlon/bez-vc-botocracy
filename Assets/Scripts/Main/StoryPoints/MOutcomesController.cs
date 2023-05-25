using System;
using System.Collections.Generic;
using Core.EventSystem;
using UnityEngine;

namespace Main.StoryPoints {
    public class MOutcomesController : MonoBehaviour {
        [Header("Outcome Prefab"), SerializeField]
        private MUIOutcome outcomePrefab;

        [Header("Outcome Container"), SerializeField]
        private RectTransform verticalContainer;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private Queue<MUIOutcome> _outcomeQueue;

        private void Awake() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }

        private void OnDestroy() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnStoryEvaluated);
        }


        #region EventHandlers

        private void OnStoryEvaluated(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            var newOutcome = Instantiate(outcomePrefab, verticalContainer);
            newOutcome.SetText(storyEventArgs.Story.Outcome);
            _outcomeQueue.Enqueue(newOutcome);
        }

        #endregion
    }
}