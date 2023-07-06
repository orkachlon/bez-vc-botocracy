using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menus.Buttons {
    public class MButtonShifter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private RectTransform _rt;
        private Tween _anim;

        private void Awake() {
            _rt = GetComponent<RectTransform>();
        }

        private void OnDestroy() {
            _anim?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (_rt == null) {
                return;
            }
            _anim?.Kill();
            _anim = _rt.DOAnchorPosX(250, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (_rt == null) {
                return;
            }
            _anim?.Kill();
            _anim = _rt.DOAnchorPosX(200, 0.2f);
        }
    }
}