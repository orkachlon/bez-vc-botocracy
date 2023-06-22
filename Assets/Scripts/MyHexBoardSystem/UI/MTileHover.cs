using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Neuron;
using ExternBoardSystem.Tools.Input.Mouse;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
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

        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        private IBoardNeuron _currentNeuron;
        private IMouseInput _mouseInput;
        private Camera _cam;
        
        private Hex? _currentTile;
        private readonly HashSet<Hex> _highlighted = new HashSet<Hex>();

        private void Awake() {
            _mouseInput = GetComponent<IMouseInput>();
            _cam = Camera.main;
        }

        private void OnEnable() {
            _mouseInput.OnPointerEnter += OnShow;
            _mouseInput.OnPointerStay += UpdatePosition;
            _mouseInput.OnPointerExit += OnHide;
            _mouseInput.OnPointerClick += OnPointerClick;
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateCurrentNeuron);
        }

        private void OnDisable() {
            _mouseInput.OnPointerEnter -= OnShow;
            _mouseInput.OnPointerStay -= UpdatePosition;
            _mouseInput.OnPointerExit -= OnHide;
            _mouseInput.OnPointerClick -= OnPointerClick;
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateCurrentNeuron);
        }

        private void UpdateCurrentNeuron(EventArgs obj) {
            if (obj is not NeuronQueueEventArgs queueArgs) {
                return;
            }

            _currentNeuron = queueArgs.NeuronQueue.NextBoardNeuron;
            if (_currentTile.HasValue) {
                Show(_currentTile.Value);
            }
        }

        #region EventHandlers

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

        #endregion

        private void Show(Hex hex) {
            if (!boardController.Board.HasPosition(hex)) {
                return;
            }

            var canBePlaced = false;
            
            if (_currentNeuron != null && !boardController.Board.GetPosition(hex).HasData()) {
                var neighbors = boardController.Manipulator.GetNeighbours(hex);
                canBePlaced = neighbors.Any(h =>
                    boardController.Board.HasPosition(h) && boardController.Board.GetPosition(h).HasData());
            }

            var tileToShow = canBePlaced ? canBePlacedTileBase : cannotBePlacedTileBase;

            boardController.SetTile(hex, tileToShow, BoardConstants.MouseHoverTileLayer);
            // boardController.SetTile(hex, hoverOutlineTile, BoardConstants.MouseHoverOutlineTileLayer);
            _currentTile = hex;

            if (canBePlaced) {
                ShowEffect(hex);
            }
        }

        private void Hide() {
            if (!_currentTile.HasValue) {
                return;
            }
            boardController.SetTile(_currentTile.Value, null, BoardConstants.MouseHoverTileLayer);
            // boardController.SetTile(_currentTile.Value, null, BoardConstants.MouseHoverOutlineTileLayer);
            _currentTile = null;

            foreach (var hex in _highlighted) {
                boardController.SetTile(hex, null, BoardConstants.MouseHoverTileLayer);
            }
            _highlighted.Clear();
        }

        private void ShowEffect(Hex hex) {
            if (_currentNeuron == null) {
                return;
            }

            var affected = _currentNeuron.GetAffectedTiles(hex, boardController);
            foreach (var effectTile in affected) {
                if (!boardController.Board.HasPosition(effectTile)) {
                    continue;
                }
                _highlighted.Add(effectTile);
                boardController.SetTile(effectTile, _currentNeuron.GetEffectTile(), BoardConstants.MouseHoverTileLayer);
            } 
        }
    }
}