using Audio;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.SP;
using System;
using System.Collections;
using System.Linq;
using Types.Trait;
using UnityEngine;

namespace Assets.Scripts.MyHexBoardSystem.Traits.TraitCompass {
    public class MTraitHoverAudioPlayer : MonoBehaviour {

        [Header("Sound"), SerializeField] private AudioClip deciderSound;
        [SerializeField] private AudioClip nonDeciderSound;
        [SerializeField, Range(0, 1)] private float volume;

        [SerializeField] private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        private ETrait[] _decidingTraits;

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassEnterStatic, PlayHoverSound);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassEnterStatic, PlayHoverSound);
        }


        private void PlayHoverSound(EventArgs args) {
            if (args is not TraitCompassHoverEventArgs hoverArgs || !hoverArgs.HighlightedTrait.HasValue) {
                MLogger.LogEditorWarning("Event args type mismatch!");
                return;
            }
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = volume;
            s.Source.PlayOneShot(_decidingTraits.Contains(hoverArgs.HighlightedTrait.Value) ? deciderSound : nonDeciderSound);
            AudioSpawner.ReleaseWhenDone(s);
        }


        private void OnInitStory(EventArgs args) {
            if (args is not StoryEventArgs spArgs) {
                MLogger.LogEditorWarning("Expected different args type!");
                return;
            }
            _decidingTraits = spArgs.Story.DecidingTraits.Keys.ToArray();
        }

    }
}