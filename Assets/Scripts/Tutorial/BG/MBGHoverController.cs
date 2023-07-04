using System;
using MyHexBoardSystem.Traits.TraitCompass;
using UnityEngine;

namespace Tutorial.BG {
    public class MBGHoverController : MonoBehaviour {

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