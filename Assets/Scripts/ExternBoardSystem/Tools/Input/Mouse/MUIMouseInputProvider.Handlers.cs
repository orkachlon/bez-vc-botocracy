using UnityEngine.EventSystems;

namespace ExternBoardSystem.Tools.Input.Mouse
{
    public partial class MUIMouseInputProvider
    {
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            ((IMouseInput) this).OnBeginDrag?.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            ((IMouseInput) this).OnDrag?.Invoke(eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            ((IMouseInput) this).OnDrop?.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            ((IMouseInput) this).OnEndDrag?.Invoke(eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ((IMouseInput) this).OnPointerClick?.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            ((IMouseInput) this).OnPointerDown?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            ((IMouseInput) this).OnPointerUp?.Invoke(eventData);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            _isInsideBoard = true;
            ((IMouseInput) this).OnPointerEnter?.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            _isInsideBoard = false;
            ((IMouseInput) this).OnPointerExit?.Invoke(eventData);
        }
    }
}