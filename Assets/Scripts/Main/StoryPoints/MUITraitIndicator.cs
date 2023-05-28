using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Main.GameStats;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.StoryPoints {
    public class MUITraitIndicator : MonoBehaviour {
        [SerializeField] private EStatType statType;

        [SerializeField] private Color addColor;
        [SerializeField] private Color subtractColor;
        
        [SerializeField] private Image baseHex;
        
        [Header("Triangles"), SerializeField] private Image topRight;
        [SerializeField] private Image topLeft;
        [SerializeField] private Image midLeft;
        [SerializeField] private Image botLeft;
        [SerializeField] private Image botRight;
        [SerializeField] private Image midRight;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        
        private List<Image> _traitIcons;

        private const int TopRightMask = 0b000001;
        private const int TopLeftMask = 0b000010;
        private const int MidLeftMask = 0b000100;
        private const int BotLeftMask = 0b001000;
        private const int BotRightMask = 0b010000;
        private const int MidRightMask = 0b100000;


        private void Awake() {
            _traitIcons = new List<Image>() {
                topRight,
                topLeft,
                midLeft,
                botLeft,
                botRight,
                midRight
            };
            
            storyEventManager.Register(StoryEvents.OnInitStory, ShowIndicator);
        }

        private void OnDestroy() {
            storyEventManager.Unregister(StoryEvents.OnInitStory, ShowIndicator);
        }

        private void ShowNegative(HashSet<ETraitType> traits) {
            baseHex.gameObject.SetActive(true);
            foreach (var trait in traits) {
                switch (trait) {
                    case ETraitType.Defender:
                        topRight.gameObject.SetActive(true);
                        topRight.color = subtractColor;
                        break;
                    case ETraitType.Commander:
                        topLeft.gameObject.SetActive(true);
                        topLeft.color = subtractColor;
                        break;
                    case ETraitType.Entrepreneur:
                        midLeft.gameObject.SetActive(true);
                        midLeft.color = subtractColor;
                        break;
                    case ETraitType.Logistician:
                        botLeft.gameObject.SetActive(true);
                        botLeft.color = subtractColor;
                        break;
                    case ETraitType.Entropist:
                        botRight.gameObject.SetActive(true);
                        botRight.color = subtractColor;
                        break;
                    case ETraitType.Mediator:
                        midRight.gameObject.SetActive(true);
                        midRight.color = subtractColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ShowPositive(HashSet<ETraitType> traits) {
            foreach (var trait in traits) {
                switch (trait) {
                    case ETraitType.Defender:
                        topRight.gameObject.SetActive(true);
                        topRight.color = addColor;
                        break;
                    case ETraitType.Commander:
                        topLeft.gameObject.SetActive(true);
                        topLeft.color = addColor;
                        break;
                    case ETraitType.Entrepreneur:
                        midLeft.gameObject.SetActive(true);
                        midLeft.color = addColor;
                        break;
                    case ETraitType.Logistician:
                        botLeft.gameObject.SetActive(true);
                        botLeft.color = addColor;
                        break;
                    case ETraitType.Entropist:
                        botRight.gameObject.SetActive(true);
                        botRight.color = addColor;
                        break;
                    case ETraitType.Mediator:
                        midRight.gameObject.SetActive(true);
                        midRight.color = addColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Show(int mask) {
            baseHex.gameObject.SetActive(true);
            topRight.gameObject.SetActive((mask & TopRightMask) > 0);
            topLeft.gameObject.SetActive((mask & TopLeftMask) > 0);
            midLeft.gameObject.SetActive((mask & MidLeftMask) > 0);
            botLeft.gameObject.SetActive((mask & BotLeftMask) > 0);
            botRight.gameObject.SetActive((mask & BotRightMask) > 0);
            midRight.gameObject.SetActive((mask & MidRightMask) > 0);
        }

        public void Hide() {
            baseHex.gameObject.SetActive(false);
            _traitIcons.ForEach(t => t.gameObject.SetActive(false));
        }

        #region EventHandlers

        private void ShowIndicator(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            var story = storyEventArgs.Story;
            var traitWeights = story.TraitWeights[statType];
            ShowNegative(traitWeights.Keys.Where(k => traitWeights[k] < 0).ToHashSet());
            ShowPositive(traitWeights.Keys.Where(k => traitWeights[k] > 0).ToHashSet());
        }

        #endregion
    }
}