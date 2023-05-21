using System;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Tools.Input.Mouse;
using Managers;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.UI {
    
    /// <summary>
    ///     This class is supposed to show the neuron which is about to be placed.
    /// </summary>
    [RequireComponent(typeof(IMouseInput))]
    public class MNeuronHover : MonoBehaviour {

        [Header("Current Neuron"), SerializeField]
        private SNeuronData currentNeuron;
        
        private IMouseInput _mouseInput;
        private Camera _cam;
        private MUIBoardNeuron _currentUINeuron;


        private void Awake() {
            _cam = Camera.main;
            _mouseInput = GetComponent<IMouseInput>();
            _mouseInput.OnPointerEnter += Show;
            _mouseInput.OnPointerStay += UpdatePosition;
            _mouseInput.OnPointerExit += Hide;
            _mouseInput.OnPointerClick += OnPointerClick;
        }

        private void OnDestroy() {
            _mouseInput.OnPointerEnter -= Show;
            _mouseInput.OnPointerStay -= UpdatePosition;
            _mouseInput.OnPointerExit -= Hide;
        }

        private void Show(PointerEventData eventData) {
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
                return;
            }
            var neuronModel = currentNeuron.GetModel();
            _currentUINeuron = MObjectPooler.Instance.Get<MUIBoardNeuron>(neuronModel.gameObject);
            _currentUINeuron.SetRuntimeElementData(currentNeuron.GetElement());
        }

        private void UpdatePosition(Vector2 screenPos) {
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
                return;
            }

            var newPos = _cam.ScreenToWorldPoint(screenPos);
            _currentUINeuron.SetWorldPosition(new Vector3(newPos.x, newPos.y, 0));
        }

        private void Hide(PointerEventData eventData) {
            if (_currentUINeuron == null) {
                return;
            }
            MObjectPooler.Instance.Release(_currentUINeuron.gameObject);
        }

        private void OnPointerClick(PointerEventData eventData) {
            Hide(eventData);
            Show(eventData);
        }
    }
}