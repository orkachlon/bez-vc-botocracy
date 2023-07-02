using Core.EventSystem;
using DG.Tweening;
using Events.Tutorial;
using MyHexBoardSystem.BoardSystem;
using MyHexBoardSystem.UI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorial.Traits.Labels {
    public class MTutorialTraitLabelController : MTraitLabelPresenter/*, IPointerExitHandler*/ {

        [SerializeField] private MTraitAccessor traitAccessor;
        [SerializeField] private int distanceFromBoard;
        [SerializeField] private int outOfScreenDistance;
        [SerializeField] private float animationDuration;
        [SerializeField] private SEventManager tutorialEventManager;

        public bool IsSPEnabled { get; set; }

        protected override async void Awake() {
            base.Awake();
            await Hide(true);
        }

        public async Task Hide(bool immediate = false) {
            var direction = traitAccessor.TraitToVectorDirection(trait).normalized;
            if (immediate) {
                textField.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0);
                textField.rectTransform.anchoredPosition = direction * outOfScreenDistance;
                textField.enabled = false;
                return;
            }
            var rt = textField.rectTransform;
            rt.anchoredPosition = direction * distanceFromBoard;
            await Task.WhenAll(
                rt.DOAnchorPos(direction * outOfScreenDistance, animationDuration).AsyncWaitForCompletion(),
                textField.DOFade(0, animationDuration).AsyncWaitForCompletion());
            textField.enabled = false;
        }

        public async Task Show(bool immediate = false) {
            textField.enabled = true;
            textField.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 1);
            if (immediate) {
                textField.rectTransform.anchoredPosition = traitAccessor.TraitToVectorDirection(trait).normalized * distanceFromBoard;
                return;
            }
            var direction = traitAccessor.TraitToVectorDirection(trait).normalized;
            var rt = textField.rectTransform;
            rt.anchoredPosition = direction * outOfScreenDistance;
            await Task.WhenAll(
                rt.DOAnchorPos(direction * distanceFromBoard, animationDuration).AsyncWaitForCompletion(),
                textField.DOFade(1, animationDuration).AsyncWaitForCompletion());
        }

        public void UpdateColor() {
            UpdateHighlightState();
        }

        public void OnPointerExit(PointerEventData eventData) {
            tutorialEventManager.Raise(TutorialEvents.OnTraitHover, new TutorialTraitHoverEventArgs(trait));
        }

        protected override void Highlight() {
            if (IsSPEnabled) {
                base.Highlight();
            } else {
                Lowlight();
            }
        }

        protected override void Lowlight() {
            if (IsSPEnabled) {
                textField.DOColor(Color.white, 0.5f);
            } else {
                textField.DOColor(_baseColor, 0.5f);
            }
        }
    }
}