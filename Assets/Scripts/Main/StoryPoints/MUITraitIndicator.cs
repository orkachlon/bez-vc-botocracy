using System;
using System.Collections.Generic;
using Main.Traits;
using UnityEngine;

namespace Main.StoryPoints {
    public class MUITraitIndicator : MonoBehaviour {
        [SerializeField] private SpriteRenderer baseColor;
        
        [Header("Triangles"), SerializeField] private SpriteRenderer topRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer midLeft;
        [SerializeField] private SpriteRenderer botLeft;
        [SerializeField] private SpriteRenderer botRight;
        [SerializeField] private SpriteRenderer midRight;

        private List<SpriteRenderer> _traitIcons;

        private const int TopRightMask = 0b000001;
        private const int TopLeftMask = 0b000010;
        private const int MidLeftMask = 0b000100;
        private const int BotLeftMask = 0b001000;
        private const int BotRightMask = 0b010000;
        private const int MidRightMask = 0b100000;


        private void Awake() {
            _traitIcons = new List<SpriteRenderer>() {
                topRight,
                topLeft,
                midLeft,
                botLeft,
                botRight,
                midRight
            };
        }

        public void Show(HashSet<ETraitType> traits) {
            baseColor.gameObject.SetActive(true);
            foreach (var trait in traits) {
                switch (trait) {
                    case ETraitType.Empathy:
                        topRight.gameObject.SetActive(true);
                        break;
                    case ETraitType.Righteousness:
                        topLeft.gameObject.SetActive(true);
                        break;
                    case ETraitType.Charisma:
                        midLeft.gameObject.SetActive(true);
                        break;
                    case ETraitType.Perception:
                        botLeft.gameObject.SetActive(true);
                        break;
                    case ETraitType.Optimism:
                        botRight.gameObject.SetActive(true);
                        break;
                    case ETraitType.Intelligence:
                        midRight.gameObject.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Show(int mask) {
            baseColor.gameObject.SetActive(true);
            topRight.gameObject.SetActive((mask & TopRightMask) > 0);
            topLeft.gameObject.SetActive((mask & TopLeftMask) > 0);
            midLeft.gameObject.SetActive((mask & MidLeftMask) > 0);
            botLeft.gameObject.SetActive((mask & BotLeftMask) > 0);
            botRight.gameObject.SetActive((mask & BotRightMask) > 0);
            midRight.gameObject.SetActive((mask & MidRightMask) > 0);
        }

        public void Hide() {
            baseColor.gameObject.SetActive(false);
            _traitIcons.ForEach(t => t.gameObject.SetActive(false));

        }
    }
}