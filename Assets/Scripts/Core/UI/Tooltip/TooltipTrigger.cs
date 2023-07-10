using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI.Tooltip {
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField] private string header;
        [TextArea(5, 8), SerializeField] private string content;

        private Sequence _showSequence;
        
        public void OnPointerEnter(PointerEventData eventData) {
            _showSequence = DOTween.Sequence().SetDelay(0.5f).AppendCallback(() => TooltipSystem.Show(content, header)).Play();
        }

        public void OnPointerExit(PointerEventData eventData) {
            _showSequence?.Kill();
            TooltipSystem.Hide();
        }
    }
}