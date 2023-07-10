using System;
using System.Linq;
using Core.EventSystem;
using Events.Board;
using MyHexBoardSystem.BoardSystem;
using Types.Trait;
using UnityEngine;

namespace MyHexBoardSystem.UI {
    public class MTraitLabelPositioner : MonoBehaviour {
        [SerializeField] private ETrait trait;
        [SerializeField] private float labelBufferFromBoard;
        [SerializeField] private Canvas canvas;

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
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, PositionInWorld);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, PositionInWorld);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, PositionInWorld);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, PositionInWorld);
        }

        private void LateUpdate() {
            PositionOnScreen();
        }

        private void PositionInWorld(EventArgs obj = null) {
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

        private void PositionOnScreen() {
            var bubbleScreenPosition = _camera.WorldToScreenPoint(_worldPos);
            //bubbleScreenPosition += new Vector3 (0, 0, 0); //This offsets it from the object, If you want it above a character head for example.
            _rt.position = bubbleScreenPosition;
            var labelRect = _rt.rect;
            var canvasScaleFactor = canvas.scaleFactor;
            var labelX = labelRect.width * canvasScaleFactor;
            var labelY = labelRect.height * canvasScaleFactor;
            var position = _rt.position;
            if (bubbleScreenPosition.y <= 0f + labelY/2) {
                position = new Vector3 (position.x, 0f + labelY/2, position.z);
            } else if (bubbleScreenPosition.y >= _camera.pixelHeight - labelY/2) {
                position = new Vector3 (position.x, _camera.pixelHeight - labelY/2, position.z);
            }
            if (bubbleScreenPosition.x <= 0f + labelX/2) {
                position = new Vector3 (0f + labelX/2, position.y, position.z);
            } else if (bubbleScreenPosition.x >= _camera.pixelWidth - labelX/2) {
                position = new Vector3 (_camera.pixelWidth - labelX/2, position.y, position.z);
            }
            _rt.position = position;
        }
    }
}