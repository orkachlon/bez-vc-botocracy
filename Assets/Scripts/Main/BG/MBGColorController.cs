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

        [Header("Colors"), SerializeField] private Color colorA;
        [SerializeField] private Color colorB;
        [SerializeField] private Color nonDecidingColor;
        [SerializeField] private float transitionDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        [Header("Data accessors"), SerializeField] private MBoardNeuronsController neuronsController;

        private readonly Dictionary<ETrait, Color> _traitCurrentColors = new Dictionary<ETrait, Color>();
        private Material _material;
        protected ETrait[] _decidingTraits;

        private void Awake() {
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

        private void UpdateBGColors(EventArgs args) {
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
            DOVirtual.Color(_traitCurrentColors[trait], DecidingTraitColorBasedOnNeurons(trait), transitionDuration, col => {
                _material.SetColor(TraitToVariableName(trait), col);
            })
            .OnComplete(() => {
                _traitCurrentColors[trait] = DecidingTraitColorBasedOnNeurons(trait);
            });
        }

        private void SetNonDecidingTraitColor(ETrait trait) {
            DOVirtual.Color(_traitCurrentColors[trait], nonDecidingColor, transitionDuration, col => {
                _material.SetColor(TraitToVariableName(trait), col);
            })
            .OnComplete(() => {
                _traitCurrentColors[trait] = nonDecidingColor;
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

        private Color TraitToDefaultColor(ETrait trait) {
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