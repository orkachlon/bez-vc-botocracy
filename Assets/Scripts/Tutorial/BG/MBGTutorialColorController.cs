using System;
using System.Linq;
using Core.EventSystem;
using DG.Tweening;
using Main.BG;
using Types.Trait;
using UnityEngine;

namespace Tutorial.BG {
    public class MBGTutorialColorController : MBGColorController {
        [SerializeField] private ETrait[] traits;
        [SerializeField] private int outlineThickness;
        [SerializeField] private Material outlineMaterial;

        [Header("Event Managers"), SerializeField] private SEventManager tutorialEventManager;

        public bool IsSPEnabled { get; set; } = false;

        public void SetDecidingTraits(ETrait[] deciders) {
            _decidingTraits = traits.Where(deciders.Contains).ToArray();
        }

        protected override void UpdateBGColors(EventArgs args) {
            if (IsSPEnabled) {
                base.UpdateBGColors(args);
            }
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

        public void ToDefaultColors(bool immediate = false) {
            if (immediate) {
                foreach (var trait in TutorialConstants.Traits) {
                    _material.SetColor(TraitToVariableName(trait), TraitToDefaultColor(trait));
                    _traitCurrentColors[trait] = TraitToDefaultColor(trait);
                }
                return;
            }
            foreach (var trait in TutorialConstants.Traits) {
                InterpolateColor(trait, _traitCurrentColors[trait], TraitToDefaultColor(trait));
            }
        }

        public void EnableLines(bool immediate = false) {
            if (immediate) {
                outlineMaterial.SetFloat("_Threshold", outlineThickness * 0.1f);
                outlineMaterial.SetFloat("_Length", -1);
                return;
            }
            outlineMaterial.SetFloat("_Threshold", outlineThickness * 0.1f);
            outlineMaterial.SetFloat("_Length", 0);
            DOVirtual.Float(0, 20, 0.5f, length => outlineMaterial.SetFloat("_Length", length));
        }

        public void DisableLines(bool immediate = false) {
            if (immediate) {
                outlineMaterial.SetFloat("_Threshold", 0);
                return;
            }
            outlineMaterial.SetFloat("_Threshold", outlineThickness * 0.1f);
            outlineMaterial.SetFloat("_Length", 20);
            DOVirtual.Float(20, 0, 0.5f, length => outlineMaterial.SetFloat("_Length", length));
        }

        protected override Color TraitToDefaultColor(ETrait trait) {
            return trait switch {
                ETrait.Protector => colorB,
                ETrait.Commander => colorB,
                ETrait.Entrepreneur => colorB,
                ETrait.Logistician => colorB,
                ETrait.Entropist => colorB,
                ETrait.Mediator => colorB,
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }

        protected override int TraitToShaderSelection(ETrait trait) {
            return trait switch {
                ETrait.Commander => 0,
                ETrait.Entrepreneur => 1,
                ETrait.Mediator => 2,
                _ => throw new NotImplementedException(),
            };
        }
    }
}