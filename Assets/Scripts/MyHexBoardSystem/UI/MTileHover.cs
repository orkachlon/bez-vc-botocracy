using System.Linq;
using ExternBoardSystem.Tools.Input.Mouse;
using MyHexBoardSystem.BoardElements.Neuron.Data;
using MyHexBoardSystem.BoardSystem;
using Types.Hex.Coordinates;
using Types.Neuron;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.UI {
    
    [RequireComponent(typeof(IMouseInput))]
    public class MTileHover : MonoBehaviour {

        [SerializeField] private MNeuronBoardController boardController;
        [SerializeField] private TileBase canBePlacedTileBase;
        [SerializeField] private TileBase cannotBePlacedTileBase;
        [SerializeField] private TileBase hoverOutlineTile;
        [SerializeField] private SNeuronDataBase currentNeuron;
        
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
            _mouseInput.OnPointerClick += OnPointerClick;
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnShow;
            _mouseInput.OnPointerStay -= UpdatePosition;
            _mouseInput.OnPointerExit -= OnHide;
            _mouseInput.OnPointerClick -= OnPointerClick;
        }

        private void OnPointerClick(PointerEventData obj) {
            OnHide(obj);
            OnShow(obj);
        }

        private void OnShow(PointerEventData eventData) {
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
            if (!boardController.Board.HasPosition(hex)) {
                return;
            }
            TileBase tileToShow;
            // no neurons or current tile is occupied
            if (ENeuronType.Undefined.Equals(currentNeuron.Type) || boardController.Board.GetPosition(hex).HasData()) {
                tileToShow = cannotBePlacedTileBase;
            }
            // current tile is empty - check if a neighbor neuron exists
            else {
                var neighbors = boardController.Manipulator.GetNeighbours(hex);
                var canBePlaced = neighbors.Any(h =>
                    boardController.Board.HasPosition(h) && boardController.Board.GetPosition(h).HasData());
                tileToShow = canBePlaced ? canBePlacedTileBase : cannotBePlacedTileBase;
            }

            boardController.SetTile(hex, tileToShow, BoardConstants.MouseHoverTileLayer);
            boardController.SetTile(hex, hoverOutlineTile, BoardConstants.MouseHoverOutlineTileLayer);
            _currentTile = hex;
        }

        private void Hide() {
            if (!_currentTile.HasValue) {
                return;
            }
            boardController.SetTile(_currentTile.Value, null, BoardConstants.MouseHoverTileLayer);
            boardController.SetTile(_currentTile.Value, null, BoardConstants.MouseHoverOutlineTileLayer);
            _currentTile = null;
        }
    }
}