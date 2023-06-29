using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Menus {
    public abstract class MMainMenuBaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        protected RectTransform _rt;

        protected virtual void Awake() {
            _rt = GetComponent<RectTransform>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData) {
            if (_rt == null) {
                return;
            }
            _rt.DOAnchorPosX(250, 0.2f);
        }

        public virtual void OnPointerExit(PointerEventData eventData) {
            if (_rt == null) {
                return;
            }
            _rt.DOAnchorPosX(200, 0.2f);
        }
    }
}