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

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Register(StoryEvents.OnEvaluate, OnSPEvaluated);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnSPEvaluated);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!HasEffect) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!HasEffect) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }

        private void OnInitStory(EventArgs obj) {
            if (obj is not StoryEventArgs spArgs) {
                return;
            }

            HasEffect = spArgs.Story.DecidingTraits.ContainsKey(trait);
        }

        private void OnSPEvaluated(EventArgs obj) {
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }
    }
}