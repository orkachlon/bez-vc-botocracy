using System.Linq;
using ExternBoardSystem.Tools;
using ExternBoardSystem.Tools.Input.Mouse;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using Main.Neurons.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.MyHexBoardSystem.BoardElements.Neuron {
    
    /// <summary>
    ///     This class is supposed to show the neuron which is about to be placed.
    /// </summary>
    [RequireComponent(typeof(IMouseInput))]
    public class MNeuronHover : MonoBehaviour {

        [Header("Board Data"), SerializeField]
        private SNeuronDataBase currentNeuron;
        [SerializeField] private MNeuronBoardController boardController;
        
        private IMouseInput _mouseInput;
        private Camera _cam;
        private MUIBoardNeuron _currentUINeuron;


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
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnShow;
            _mouseInput.OnPointerStay -= OnUpdatePosition;
            _mouseInput.OnPointerExit -= OnHide;
        }

        #endregion

        #region EventHandlers

        private void OnShow(PointerEventData eventData) {
            // check that we have a neuron
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
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
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
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

        #endregion

        private void Show() {
            if (_currentUINeuron != null) {
                return;
            }
            var neuronModel = currentNeuron.GetModel();
            _currentUINeuron = MObjectPooler.Instance.Get<MUIBoardNeuron>(neuronModel.gameObject);
            _currentUINeuron.SetRuntimeElementData(currentNeuron.GetElement());
            _currentUINeuron.ToHoverLayer();
        }

        private void Hide() {
            if (_currentUINeuron == null) {
                return;
            }
            _currentUINeuron.ToBoardLayer();
            MObjectPooler.Instance.Release(_currentUINeuron.gameObject);
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