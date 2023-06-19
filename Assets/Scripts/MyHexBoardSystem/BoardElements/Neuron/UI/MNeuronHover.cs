using System;
using System.Linq;
using Core.EventSystem;
using Core.Tools.Pooling;
using ExternBoardSystem.Tools.Input.Mouse;
using MyHexBoardSystem.BoardSystem;
using Neurons;
using Types.Board.UI;
using Types.Neuron.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.BoardElements.Neuron.UI {
    
    /// <summary>
    ///     This class is supposed to show the neuron which is about to be placed.
    /// </summary>
    [RequireComponent(typeof(IMouseInput))]
    public class MNeuronHover : MonoBehaviour {
        
        [SerializeField] private MNeuronBoardController boardController;
        [Header("Neuron Board Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        private IBoardNeuron _currentNeuron;
        private IMouseInput _mouseInput;
        private Camera _cam;
        private IUIBoardNeuron _currentUINeuron;


        #region UnityMethods

        private void Awake() {
            _cam = Camera.main;
            _mouseInput = GetComponent<IMouseInput>();
        }

        private void OnEnable() {
            _mouseInput.OnPointerEnter += OnShow;
            _mouseInput.OnPointerStay += OnUpdatePosition;
            _mouseInput.OnPointerExit += OnHide;
            _mouseInput.OnPointerClick += OnPointerClick;
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnShow;
            _mouseInput.OnPointerStay -= OnUpdatePosition;
            _mouseInput.OnPointerExit -= OnHide;
            _mouseInput.OnPointerClick -= OnPointerClick;
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        #endregion

        #region EventHandlers

        private void OnShow(PointerEventData eventData) {
            // check that we have a neuron
            if (_currentNeuron == null) {
                return;
            }
            // check if placement is legal
            if (!IsLegalPlacement(eventData.position)) {
                return;
            }
            // show neuron
            Show();
        }

        private void OnUpdatePosition(Vector2 screenPos) {
            // check that we have a neuron
            if (_currentNeuron == null) {
                return;
            }
            // check if placement is legal
            if (!IsLegalPlacement(screenPos)) {
                Hide();
                return;
            }
            
            Show();
            var newPos = _cam.ScreenToWorldPoint(screenPos);
            _currentUINeuron.SetWorldPosition(new Vector3(newPos.x, newPos.y, 0));
        }

        private void OnHide(PointerEventData eventData) {
            Hide();
        }

        private void OnPointerClick(PointerEventData eventData) {
            OnHide(eventData);
            OnShow(eventData);
        }
        
        private void UpdateNextNeuron(EventArgs obj) {
            if (obj is not NeuronQueueEventArgs queueEventArgs) {
                return;
            }

            _currentNeuron = queueEventArgs.NeuronQueue.NextBoardNeuron;
        }

        #endregion

        private void Show() {
            if (_currentUINeuron != null) {
                return;
            }

            _currentUINeuron = _currentNeuron.Pool();
            _currentUINeuron.PlayHoverAnimation();
            _currentUINeuron.ToHoverLayer();
        }

        private void Hide() {
            if (_currentUINeuron == null) {
                return;
            }
            _currentUINeuron.StopHoverAnimation();
            _currentUINeuron.ToBoardLayer();
            MObjectPooler.Instance.Release(_currentUINeuron.GO);
            _currentUINeuron = null;
        }

        private bool IsLegalPlacement(Vector2 screenPos) {
            var mouseWorld = _cam.ScreenToWorldPoint(screenPos);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            return boardController.Board.HasPosition(mouseHex) &&
                   !boardController.Board.GetPosition(mouseHex).HasData() &&
                   boardController.Manipulator.GetNeighbours(mouseHex)
                       .Any(h => boardController.Board.HasPosition(h) 
                                 && boardController.Board.GetPosition(h).HasData());
        }
    }
}