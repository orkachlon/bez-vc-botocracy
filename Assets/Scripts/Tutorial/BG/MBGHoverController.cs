using Core.EventSystem;
using MyHexBoardSystem.Traits.TraitCompass;
using System;
using UnityEngine;

namespace Assets.Scripts.Tutorial.BG {
    public class MBGHoverController : MonoBehaviour {

        [SerializeField] private SEventManager tutorialEventManager;
        [SerializeField] private MBGCompassDirection[] hoverAreas;

        public void EnableHover(EventArgs args = null) {
            foreach (var area in hoverAreas) {
                area.IsEnabled = true;
            }
        }

        public void DisableHover(EventArgs args = null) {
            foreach (var area in hoverAreas) {
                area.IsEnabled = false;
            }
        }
    }
}