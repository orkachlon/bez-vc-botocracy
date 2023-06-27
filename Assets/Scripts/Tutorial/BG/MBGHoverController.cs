using Core.EventSystem;
using Events.Tutorial;
using System;
using UnityEngine;

namespace Assets.Scripts.Tutorial.BG {
    public class MBGHoverController : MonoBehaviour {

        [SerializeField] private SEventManager tutorialEventManager;


        private Collider2D[] hoverAreas;

        private void Awake() {
            hoverAreas = GetComponentsInChildren<Collider2D>();
        }

        private void OnEnable() {
            tutorialEventManager.Register(TutorialEvents.OnEnableBGInteraction, EnableHover);
            tutorialEventManager.Register(TutorialEvents.OnDisableBGInteraction, DisableHover);
        }

        private void OnDisable() {
            tutorialEventManager.Unregister(TutorialEvents.OnEnableBGInteraction, EnableHover);
            tutorialEventManager.Unregister(TutorialEvents.OnDisableBGInteraction, DisableHover);
        }

        private void EnableHover(EventArgs args = null) {
            foreach(var area in hoverAreas) {
                area.enabled = true;
            }
        }

        private void DisableHover(EventArgs args = null) {
            foreach (var area in hoverAreas) {
                area.enabled = false;
            }
        }
    }
}