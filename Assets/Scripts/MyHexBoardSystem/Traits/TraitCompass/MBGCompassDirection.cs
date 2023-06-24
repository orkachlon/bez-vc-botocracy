using System;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using Types.Trait;
using UnityEngine;

namespace MyHexBoardSystem.Traits.TraitCompass {
    public class MBGCompassDirection : MonoBehaviour {

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

        private void OnMouseEnter() {
            if (!HasEffect) {
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
        }

        private void OnMouseExit() {
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