using Core.EventSystem;
using Events.Board;
using ExternBoardSystem.Tools.Input.Mouse;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.Input {
    [RequireComponent(typeof(IMouseInput))]
    public class MBoardEventDelegator : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private IMouseInput _mouseInput;
        
        private void Awake() {
            _mouseInput = GetComponent<IMouseInput>();
        }

        private void OnEnable() {
            _mouseInput.OnPointerDown += OnPointerDown;
            _mouseInput.OnPointerUp += OnPointerUp;
            _mouseInput.OnPointerClick += OnPointerClick;
            _mouseInput.OnBeginDrag += OnBeginDrag;
            _mouseInput.OnDrag += OnDrag;
            _mouseInput.OnEndDrag += OnEndDrag;
            _mouseInput.OnDrop += OnDrop;
            _mouseInput.OnPointerEnter += OnPointerEnter;
            _mouseInput.OnPointerExit += OnPointerExit;
            _mouseInput.OnPointerStay += OnPointerStay;
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnPointerEnter;
        }

        #region EventDelegators
        
        private void OnPointerDown(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerDown, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnPointerUp(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerUp, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnPointerClick(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerClick, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnBeginDrag(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnBeginDrag, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnDrag(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnDrag, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnEndDrag(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnEndDrag, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnDrop(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnDrop, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnPointerEnter(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerEnter, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnPointerExit(PointerEventData pointerEventData) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerExit, new OnBoardInputEventArgs(pointerEventData));
        }
        
        private void OnPointerStay(Vector2 mousePosition) {
            boardEventManager.Raise(ExternalBoardEvents.OnPointerStay, new OnBoardPointerStayEventArgs(mousePosition));
        }

        #endregion
    }
}