using System;
using System.Linq;
using Core.EventSystem;
using Events.Tutorial;
using Main.BG;
using Types.Trait;
using UnityEngine;

namespace Tutorial.BG {
    public class MBGTutorialColorController : MBGColorController {
        [SerializeField] private ETrait[] traits;
        [SerializeField] private int outlineThickness;
        [SerializeField] private Material outlineMaterial;

        [Header("Event Managers"), SerializeField] private SEventManager tutorialEventManager;

        protected override void Awake() {
            base.Awake();
        }

        protected override void OnEnable() {
            tutorialEventManager.Register(TutorialEvents.OnEnableBGInteraction, EnableLines);
            tutorialEventManager.Register(TutorialEvents.OnDisableBGInteraction, DisableLines);
        }

        protected override void OnDisable() {
            tutorialEventManager.Unregister(TutorialEvents.OnEnableBGInteraction, EnableLines);
            tutorialEventManager.Unregister(TutorialEvents.OnDisableBGInteraction, DisableLines);
        }

        public void SetDecidingTraits(ETrait[] deciders) {
            _decidingTraits = traits.Where(deciders.Contains).ToArray();
        }
        
        protected override void ColorBG() {
            foreach (var trait in traits) {
                SetTraitBGColor(trait, _decidingTraits.Contains(trait));
            }
        }

        protected override string TraitToVariableName(ETrait trait) {
            return trait switch {
                ETrait.Protector => null,
                ETrait.Commander => "_ColorTopLeft",
                ETrait.Entrepreneur => "_ColorBot",
                ETrait.Logistician => null,
                ETrait.Entropist => null,
                ETrait.Mediator => "_ColorTopRight",
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }

        private void EnableLines(EventArgs args = null) {
            outlineMaterial.SetFloat("_Threshold", outlineThickness * 0.1f);
        }

        private void DisableLines(EventArgs args = null) {
            outlineMaterial.SetFloat("_Threshold", 0);
        }
    }
}