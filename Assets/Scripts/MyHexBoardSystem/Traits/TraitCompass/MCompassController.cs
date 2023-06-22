using System;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using UnityEngine;

namespace MyHexBoardSystem.Traits.TraitCompass {
    public class MCompassController : MonoBehaviour {

        [SerializeField] private MTraitCompass traitCompass;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        [SerializeField] private SEventManager boardEventManager;
        
        
        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, SetCurrentDecidingTrait);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, SetCurrentDecidingTrait);
        }

        private void Update() {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse2)) {
                if (traitCompass.gameObject.activeInHierarchy) {
                    traitCompass.Hide();
                }
                else {
                    traitCompass.Show(UnityEngine.Input.mousePosition);
                }
            }
        }

        private void OnInitStory(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs args) {
                return;
            }
            var sp = args.Story;
            traitCompass.SetDecidingTraits(sp.DecidingTraits.Keys);
        }

        private void SetCurrentDecidingTrait(EventArgs obj) {
            if (obj is not BoardStateEventArgs boardArgs) {
                return;
            }

            traitCompass.SetCurrentDecidingTraits(boardArgs.ElementsController.GetMaxTrait());
        }
    }
}