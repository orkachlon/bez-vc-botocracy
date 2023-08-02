using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Board;
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

        [SerializeField] protected MNeuronBoardController boardController;
        [SerializeField] protected TileBase canBePlacedTileBase;
        [SerializeField] protected TileBase cannotBePlacedTileBase;

        [Header("Event Managers"), SerializeField]
        protected SEventManager neuronEventManager;
        [SerializeField] protected SEventManager boardEventManager;
        
        private IBoardNeuron _currentNeuron;
        private IMouseInput _mouseInput;
        protected Camera Cam;
        
        protected Hex? CurrentTile;
        protected readonly HashSet<Hex> Highlighted = new HashSet<Hex>();

        protected virtual void Awake() {
            _mouseInput = GetComponent<IMouseInput>();
            Cam = Camera.main;
        }

        protected virtual void OnEnable() {
            _mouseInput.OnPointerEnter += OnShow;
            _mouseInput.OnPointerStay += UpdatePosition;
            _mouseInput.OnPointerExit += OnHide;
            _mouseInput.OnPointerClick += OnPointerClick;
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateCurrentNeuron);
        }

        protected virtual void OnDisable() {
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
            if (CurrentTile.HasValue) {
                Show(CurrentTile.Value);
            }
        }

        #region EventHandlers

        private void OnPointerClick(PointerEventData obj) {
            OnHide(obj);
        }

        private void OnShow(PointerEventData eventData) {
            var mouseWorld = Cam.ScreenToWorldPoint(eventData.position);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            Show(mouseHex);
        }

        protected virtual void UpdatePosition(Vector2 mouseScreen) {
            var mouseWorld = Cam.ScreenToWorldPoint(mouseScreen);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            if (mouseHex == CurrentTile) {
                return;
            }
            Hide();
            Show(mouseHex);
        }

        private void OnHide(PointerEventData eventData) {
            Hide();
        }

        #endregion

        protected virtual void Show(Hex hex) {
            if (!boardController.Board.HasPosition(hex)) {
                return;
            }

            var canBePlaced = false;
            
            if (_currentNeuron != null && !boardController.Board.GetPosition(hex).HasData() && boardController.Board.GetPosition(hex).IsEnabled) {
                var neighbors = boardController.Manipulator.GetNeighbours(hex);
                canBePlaced = neighbors.Any(h =>
                    boardController.Board.HasPosition(h) && boardController.Board.GetPosition(h).HasData());
            }

            var tileToShow = canBePlaced ? canBePlacedTileBase : cannotBePlacedTileBase;

            boardController.SetTile(hex, tileToShow, BoardConstants.MouseHoverTileLayer);
            CurrentTile = hex;

            boardEventManager.Raise(ExternalBoardEvents.OnTileHover, new TileHoverArgs(hex, canBePlaced));
            
            if (canBePlaced) {
                ShowEffect(hex);
            }
        }

        protected virtual void Hide() {
            if (CurrentTile.HasValue) {
                boardController.SetTile(CurrentTile.Value, null, BoardConstants.MouseHoverTileLayer);
                CurrentTile = null;
            }

            HideEffect();
        }

        protected virtual void HideEffect() {
            if (Highlighted.Count <= 0) {
                return;
            }
            foreach (var hex in Highlighted) {
                boardController.SetTile(hex, null, BoardConstants.MouseHoverTileLayer);
            }

            Highlighted.Clear();
        }

        protected virtual void ShowEffect(Hex hex) {
            if (_currentNeuron == null) {
                return;
            }

            var affected = _currentNeuron.GetAffectedTiles(hex, boardController);
            foreach (var effectTile in affected) {
                if (!boardController.Board.HasPosition(effectTile)) {
                    continue;
                }
                Highlighted.Add(effectTile);
                boardController.SetTile(effectTile, _currentNeuron.GetEffectTile(), BoardConstants.MouseHoverTileLayer);
            } 
        }
    }
}