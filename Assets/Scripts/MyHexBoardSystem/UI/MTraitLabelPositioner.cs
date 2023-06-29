using System;
using System.Linq;
using Core.EventSystem;
using Events.Board;
using MyHexBoardSystem.BoardSystem;
using Types.Board;
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
        private RectTransform _bounds;
        private Vector3 _worldPos;
        private Vector3 _traitDirection;

        private void Awake() {
            _camera = Camera.main;
            _rt = GetComponent<RectTransform>();
            _bounds = _rt.parent.GetComponent<RectTransform>();
            _traitDirection = traitAccessor.TraitToVectorDirection(trait).normalized;
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, RepositionLabel);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, RepositionLabel);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, RepositionLabel);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, RepositionLabel);
        }

        private void LateUpdate() {
            var screenPos = _camera.WorldToScreenPoint(_worldPos);
            // if (!IsInsideBounds(screenPos)) {
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
        
        private bool IsInsideBounds(Vector2 screenPos) {
            var h = _rt.sizeDelta.y * 0.5f;
            var w = _rt.sizeDelta.x * 0.5f;
            var yExtent = _bounds.sizeDelta.y * 0.5f;
            var xExtent = _bounds.sizeDelta.x * 0.5f;
            var boundsPos = _bounds.position;
            return boundsPos.y + yExtent >= screenPos.y + h && boundsPos.y - yExtent <= screenPos.y - h &&
                   boundsPos.x + xExtent >= screenPos.x + w && boundsPos.x - xExtent <= screenPos.x - w;
        }
        
         // the correct way to do this is to move the label on the worldPos -> camWorldPos vector.
        private Vector3 ShiftIntoCameraBounds(Vector3 screenPos) {
            var h = _rt.sizeDelta.y * 0.5f;
            var w = _rt.sizeDelta.x * 0.5f;
            var yExtent = _bounds.sizeDelta.y * 0.5f;
            var xExtent = _bounds.sizeDelta.x * 0.5f;
            var boundsPos = _bounds.position;
            var dir = boundsPos - _rt.position;
            if (boundsPos.y + yExtent < screenPos.y + h) {
                var yMag = screenPos.y + h - boundsPos.y + yExtent;
                var cosA = Vector3.Dot(Vector3.down, dir) / dir.magnitude;
                var a = yMag / cosA;
                screenPos += a * dir.normalized;
            } else if (boundsPos.y - yExtent > screenPos.y - h) {
                var yMag = boundsPos.y + yExtent - screenPos.y + h;
                var cosA = Vector3.Dot(Vector3.up, dir) / dir.magnitude;
                var a = yMag / cosA;
                screenPos += a * dir.normalized;
            }
            if (boundsPos.x + xExtent < screenPos.x + w) {
                var xMag = boundsPos.x + xExtent - screenPos.x + w;
                var cosA = Vector3.Dot(Vector3.left, dir) / dir.magnitude;
                var a = xMag / cosA;
                screenPos += a * dir.normalized;
            } else if (boundsPos.x + xExtent > screenPos.x - w) {
                var xMag = screenPos.x + w - boundsPos.x + xExtent;
                var cosA = Vector3.Dot(Vector3.right, dir) / dir.magnitude;
                var a = xMag / cosA;
                screenPos += a * dir.normalized;
            }
            return screenPos;
        }
    }
}