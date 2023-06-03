using System;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Tools.Input.Mouse;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.Neurons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.UI {
    
    [RequireComponent(typeof(IMouseInput))]
    public class MTileHover : MonoBehaviour {

        [SerializeField] private MNeuronBoardController boardController;
        [SerializeField] private TileBase hoverTile;
        [SerializeField] private SNeuronData currentNeuron;
        
        private IMouseInput _mouseInput;
        private Camera _cam;
        
        private Hex? _currentTile;

        private void Awake() {
            _mouseInput = GetComponent<IMouseInput>();
            _cam = Camera.main;
        }

        private void OnEnable() {
            _mouseInput.OnPointerEnter += OnShow;
            _mouseInput.OnPointerStay += UpdatePosition;
            _mouseInput.OnPointerExit += OnHide;
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnShow;
            _mouseInput.OnPointerStay -= UpdatePosition;
            _mouseInput.OnPointerExit -= OnHide;
        }

        private void OnShow(PointerEventData eventData) {
            if (ENeuronType.Undefined.Equals(currentNeuron.Type)) {
                return;
            }

            var mouseWorld = _cam.ScreenToWorldPoint(eventData.position);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            Show(mouseHex);
        }

        private void UpdatePosition(Vector2 mouseScreen) {
            var mouseWorld = _cam.ScreenToWorldPoint(mouseScreen);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            if (mouseHex == _currentTile) {
                return;
            }
            Hide();
            Show(mouseHex);
        }

        private void OnHide(PointerEventData eventData) {
            Hide();
        }

        private void Show(Hex hex) {
            boardController.SetTile(hex, hoverTile, BoardConstants.MouseHoverTileLayer);
            _currentTile = hex;
        }

        private void Hide() {
            if (!_currentTile.HasValue) {
                return;
            }
            boardController.SetTile(_currentTile.Value, null, BoardConstants.MouseHoverTileLayer);
            _currentTile = null;
        }
    }
}