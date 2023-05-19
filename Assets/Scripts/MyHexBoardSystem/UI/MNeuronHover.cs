using System;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Tools.Input.Mouse;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.UI {
    
    /// <summary>
    ///     This class is supposed to show the neuron which is about to be placed.
    /// </summary>
    [RequireComponent(typeof(IMouseInput))]
    public class MNeuronHover : MonoBehaviour {
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
            var currentNeuron = NeuronManager.Instance.CurrentNeuron;
            if (currentNeuron == null) {
                return;
            }
            var neuronModel = currentNeuron.ElementData.GetModel();
            _currentUINeuron = MObjectPooler.Instance.Get<MUIBoardNeuron>(neuronModel.gameObject);
            _currentUINeuron.SetRuntimeElementData(currentNeuron);
        }

        private void UpdatePosition(Vector2 screenPos) {
            if (_currentUINeuron == null) {
                return;
            }
            _currentUINeuron.SetWorldPosition(_cam.ScreenToWorldPoint(screenPos));
        }

        private void Hide(PointerEventData eventData) {
            MObjectPooler.Instance.Release(_currentUINeuron.gameObject);
            _currentUINeuron = null;
        }

        private void OnPointerClick(PointerEventData eventData) {
            Hide(eventData);
            Show(eventData);
        }
    }
}