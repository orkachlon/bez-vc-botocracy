using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils;
using DG.Tweening;
using Events.SP;
using Types.Trait;
using UnityEngine;

namespace Main.BG {
    public class MBGColorController : MonoBehaviour {

        [Header("Colors"), SerializeField] private Color colorA;
        [SerializeField] private Color colorB;
        [SerializeField] private Color nonDecidingColor;
        [SerializeField] private float transitionDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private Material _material;
        private readonly Dictionary<ETrait, Color> _traitCurrentColors = new Dictionary<ETrait, Color>();

        private void Awake() {
            _material = GetComponent<Renderer>().material;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                _traitCurrentColors[trait] = TraitToDefaultColor(trait);
            }
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnInitStory, SetBGColors);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, SetBGColors);
        }

        private void SetBGColors(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }

            var deciders = storyEventArgs.Story.DecidingTraits;
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                SetTraitBGColor(trait, deciders.ContainsKey(trait));
            }
        }

        private void SetTraitBGColor(ETrait trait, bool isDeciding) {
            if (!isDeciding) {
                DOVirtual.Color(_traitCurrentColors[trait], nonDecidingColor, transitionDuration, col => {
                    _material.SetColor(TraitToVariableName(trait), col);
                })
                    .OnComplete(() => {
                        _traitCurrentColors[trait] = nonDecidingColor;
                    });
                return;
            }
            DOVirtual.Color(_traitCurrentColors[trait], TraitToDefaultColor(trait), transitionDuration, col => {
                _material.SetColor(TraitToVariableName(trait), col);
            })
            .OnComplete(() => {
                _traitCurrentColors[trait] = TraitToDefaultColor(trait);
            });
        }

        private static string TraitToVariableName(ETrait trait) {
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