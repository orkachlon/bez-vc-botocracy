using System;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using Types.Trait;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.Traits.TraitCompass {
    public class MBGCompassDirection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField] private ETrait trait;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;


        private bool HasEffect { get; set; }
        private bool IsEnabled { get; set; } = true;

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnBeforeEvaluate, OnBeforeEvaluate);
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnBeforeEvaluate, OnBeforeEvaluate);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!HasEffect || !IsEnabled) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!HasEffect || !IsEnabled) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }

        #region EventHandlers
        private void OnInitStory(EventArgs obj) {
            if (obj is not StoryEventArgs spArgs) {
                return;
            }

            HasEffect = spArgs.Story.DecidingTraits.ContainsKey(trait);
        }
        private void OnBeforeEvaluate(EventArgs obj) {
            IsEnabled = false;
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }

        private void OnBoardModified(EventArgs obj) {
            IsEnabled = true;
        }


        #endregion
    }
}