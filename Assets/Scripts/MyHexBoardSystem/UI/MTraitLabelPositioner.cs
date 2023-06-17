using System;
using System.Linq;
using Core.EventSystem;
using MyHexBoardSystem.BoardSystem;
using MyHexBoardSystem.BoardSystem.Interfaces;
using MyHexBoardSystem.Events;
using Types.Trait;
using UnityEngine;

namespace MyHexBoardSystem.UI {
    public class MTraitLabelPositioner : MonoBehaviour {
        [SerializeField] private ETrait trait;
        [SerializeField] private float labelBufferFromBoard;

        [SerializeField] private MTraitAccessor traitAccessor;
        [SerializeField] private MNeuronBoardController controller;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        
        private Camera _camera;
        private RectTransform _rt;
        private Vector3 _worldPos;
        private Vector3 _traitDirection;

        private void Awake() {
            _camera = Camera.main;
            _rt = GetComponent<RectTransform>();
            _traitDirection = ITraitAccessor.TraitToVectorDirection(trait).normalized;
        }

        private void Start() {
            RepositionLabel();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, RepositionLabel);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, RepositionLabel);
        }

        private void LateUpdate() {
            var screenPos = _camera.WorldToScreenPoint(_worldPos);
            // if (IsOutSideCameraBounds(screenPos)) {
            //     screenPos = ShiftIntoCameraBounds(screenPos);
            // }
            _rt.position = screenPos;
        }

        private void RepositionLabel(EventArgs obj = null) {
            var edgeHexes = traitAccessor.GetTraitEdgeHexes(trait);
            if (edgeHexes.Length == 0) {
                _worldPos = _traitDirection * labelBufferFromBoard;
                return;
            }
            var asVectors = edgeHexes.Select(h => controller.HexToWorldPos(h)).ToArray();
            var maxMag = asVectors.Max(v => v.magnitude);
            var maxVec = asVectors.First(v => Math.Abs(v.magnitude - maxMag) < 0.1f);
            var maxPosProjected = Vector3.Project(maxVec, _traitDirection);
            
            var worldPos = maxPosProjected + _traitDirection * labelBufferFromBoard;
            _worldPos = worldPos;
        }
        //
        // private bool IsOutSideCameraBounds(Vector2 screenPos) {
        //     var h = _rt.sizeDelta.y;
        //     var w = _rt.sizeDelta.x;
        //     var camRect = _camera.pixelRect;
        //     return camRect.height < screenPos.y + h || 0 > screenPos.y - h || camRect.x < screenPos.x + w || 0 > screenPos.x - w;
        // }
        //
        //  // the correct way to do this is to move the label on the worldPos -> camWorldPos vector.
        // private Vector3 ShiftIntoCameraBounds(Vector3 screenPos) {
        //     var h = _rt.sizeDelta.y / 2;
        //     var w = _rt.sizeDelta.x / 2;
        //     var camRect = _camera.pixelRect;
        //     var shiftDelta = 0f;
        //     if (camRect.height < screenPos.y + h) {
        //         shiftDelta = screenPos.y + h - camRect.height;
        //         screenPos += Vector3.Project(Vector3.down * shiftDelta, -_traitDirection);
        //     } else if (0 > screenPos.y - h) {
        //         shiftDelta = -(screenPos.y - h);
        //         screenPos += Vector3.Project(Vector3.up * shiftDelta, -_traitDirection);
        //     }
        //     if (camRect.width < screenPos.x + w) {
        //         shiftDelta = screenPos.x + w - camRect.width;
        //         screenPos += Vector3.Project(Vector3.left * shiftDelta, -_traitDirection);
        //     } else if (0 > screenPos.x - w) {
        //         shiftDelta = -(screenPos.x - w);
        //         screenPos += Vector3.Project(Vector3.right * shiftDelta, -_traitDirection);
        //     }
        //     return screenPos;
        // }
    }
}