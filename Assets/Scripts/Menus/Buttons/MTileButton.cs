using Core.EventSystem;
using DG.Tweening;
using Events.Board;
using Events.Menu;
using ExternBoardSystem.Tools.Input.Mouse;
using Menus.MenuBoard;
using MyHexBoardSystem.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Menus.Buttons {
    public class MTileButton : MonoBehaviour {
        [SerializeField] private MMenuBoardController controller;
        [SerializeField] private List<Transform> buttonTilePositions;
        [SerializeField] private float letterShiftAmount;
        [SerializeField] private float letterShiftDuration;

        [SerializeField] private UnityEvent OnButtonDown;

        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager menuEventManager;

        public bool IsPressed { get; private set; } = false;
        public bool IsActive { get; private set; } = false;

        private Camera _cam;
        private readonly List<Tween> _tweens = new();

        private void Awake() {
            _cam = Camera.main;
            Hide();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnPointerClick, OnPointerClick);
            menuEventManager.Register(MenuEvents.OnBoardAnimated, OnShow);
        }


        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPointerClick, OnPointerClick);
            menuEventManager.Unregister(MenuEvents.OnBoardAnimated, OnShow);
        }

        private void OnDestroy() {
            foreach (var t in _tweens) {
                t?.Complete();
                t?.Kill();
            }
            _tweens.Clear();
        }

        private void OnShow(EventArgs args) {
            foreach (var l in buttonTilePositions) {
                l.gameObject.SetActive(true);
            }
            IsActive = true;
        }

        private void Hide() {
            foreach (var l in buttonTilePositions) {
                l.gameObject.SetActive(false);
            }
        }

        private void OnPointerClick(EventArgs args) {
            if (args is not OnBoardInputEventArgs inputArgs || !IsActive) {
                return;
            }
            var mouseWorld = _cam.ScreenToWorldPoint(inputArgs.EventData.position);
            if (!GetTiles().Contains(controller.WorldPosToHex(mouseWorld))) {
                return;
            }
            foreach (Hex h in GetTiles()) {
                boardEventManager.Raise(ExternalBoardEvents.OnTileOccupied, new BoardElementEventArgs<IBoardNeuron>(null, h));
            }
            foreach (var t in buttonTilePositions) {
                //_tweens.Add(t.DOMoveY(t.position.y - letterShiftAmount, letterShiftDuration));
                var currPos = t.position;
                t.position = new Vector3(currPos.x, currPos.y - letterShiftAmount, currPos.z);
            }
            IsPressed = true;
            OnButtonDown?.Invoke();
        }

        public bool Contains(Hex pos) {
            if (buttonTilePositions.Count == 0) { return false; }
            return buttonTilePositions.Any(t => controller.WorldPosToHex(t.position) == pos);
        }

        public IEnumerable<Hex> GetTiles() {
            return buttonTilePositions.Select(t => controller.WorldPosToHex(t.position));
        }
    }
}