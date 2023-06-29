using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using DG.Tweening;
using Events.Board;
using Events.SP;
using MyHexBoardSystem.BoardElements;
using Types.Trait;
using UnityEngine;
using UnityEngine.Assertions;

namespace Main.BG {
    public class MBGColorController : MonoBehaviour {

        [Header("Colors"), SerializeField] protected Color colorA;
        [SerializeField] protected Color colorB;
        [SerializeField] private Color nonDecidingColor;
        [SerializeField] private float transitionDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        [Header("Data accessors"), SerializeField] private MBoardNeuronsController neuronsController;

        protected readonly Dictionary<ETrait, Color> _traitCurrentColors = new Dictionary<ETrait, Color>();
        protected Material _material;
        protected ETrait[] _decidingTraits;

        protected virtual void Awake() {
            _material = GetComponent<Renderer>().material;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                _traitCurrentColors[trait] = TraitToDefaultColor(trait);
            }
        }

        protected virtual void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, UpdateCurrentSP);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, UpdateBGColors);
        }

        protected virtual void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, UpdateCurrentSP);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, UpdateBGColors);
        }

        private void UpdateCurrentSP(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }

            _decidingTraits = storyEventArgs.Story.DecidingTraits.Keys.ToArray();
            ColorBG();
        }

        protected virtual void UpdateBGColors(EventArgs args) {
            ColorBG();
        }

        protected virtual void ColorBG() {
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                SetTraitBGColor(trait, _decidingTraits.Contains(trait));
            }
        }

        protected void SetTraitBGColor(ETrait trait, bool isDeciding) {
            if (!isDeciding) {
                SetNonDecidingTraitColor(trait);
                return;
            }
            SetDecidingTraitColor(trait);
        }

        private void SetDecidingTraitColor(ETrait trait) {
            InterpolateColor(trait, _traitCurrentColors[trait], DecidingTraitColorBasedOnNeurons(trait))
            .OnComplete(() => {
                _traitCurrentColors[trait] = DecidingTraitColorBasedOnNeurons(trait);
            });
        }

        private void SetNonDecidingTraitColor(ETrait trait) {
            InterpolateColor(trait, _traitCurrentColors[trait], nonDecidingColor)
            .OnComplete(() => {
                _traitCurrentColors[trait] = nonDecidingColor;
            });
        }

        protected Tweener InterpolateColor(ETrait trait, Color from, Color to) {
            return DOVirtual.Color(from, to, transitionDuration, col => {
                _material.SetColor(TraitToVariableName(trait), col);
            });
        }

        private Color DecidingTraitColorBasedOnNeurons(ETrait trait) {
            var amounts = _decidingTraits
                .Select(t => neuronsController.GetTraitCount(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();
#if UNITY_EDITOR
            Assert.IsTrue(amounts.Count >= 1, $"Error collecting trait neuron amounts in {this.GetType()}");
#endif
            if (amounts.Count == 1) {
                return colorB;
            }
            var me = amounts.IndexOf(neuronsController.GetTraitCount(trait));
            return Color.Lerp(colorA, colorB, (float) me / (amounts.Count - 1));
        }

        protected virtual string TraitToVariableName(ETrait trait) {
            return trait switch {
                ETrait.Protector => "_ColorBotRight",
                ETrait.Commander => "_ColorTopLeft",
                ETrait.Entrepreneur => "_ColorLeft",
                ETrait.Logistician => "_ColorBotLeft",
                ETrait.Entropist => "_ColorTopRight",
                ETrait.Mediator => "_ColorRight",
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }

        protected virtual Color TraitToDefaultColor(ETrait trait) {
            return trait switch {
                ETrait.Protector => colorA,
                ETrait.Commander => colorB,
                ETrait.Entrepreneur => colorA,
                ETrait.Logistician => colorB,
                ETrait.Entropist => colorA,
                ETrait.Mediator => colorB,
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }
    }
}