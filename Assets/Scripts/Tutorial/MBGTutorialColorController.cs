using System;
using System.Linq;
using Main.BG;
using Types.Trait;
using UnityEngine;

namespace Tutorial {
    public class MBGTutorialColorController : MBGColorController {
        [SerializeField] private ETrait[] traits;

        protected override void OnEnable() { }

        protected override void OnDisable() { }

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
    }
}