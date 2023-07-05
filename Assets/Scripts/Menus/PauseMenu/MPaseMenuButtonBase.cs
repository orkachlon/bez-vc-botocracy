using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.UI.PauseMenu {
    public class MPaseMenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        protected RectTransform RT;
        
        protected virtual void Awake() {
            RT = GetComponent<RectTransform>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData) {
            if (RT == null) {
                return;
            }
            RT.DOAnchorPosX(250, 0.2f);
        }

        public virtual void OnPointerExit(PointerEventData eventData) {
            if (RT == null) {
                return;
            }
            RT.DOAnchorPosX(200, 0.2f);
        }
    }
}