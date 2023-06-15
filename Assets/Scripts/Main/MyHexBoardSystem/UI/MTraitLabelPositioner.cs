using System;
using System.Linq;
using Core.EventSystem;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Traits;
using UnityEngine;

namespace Main.MyHexBoardSystem.UI {
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
            _traitDirection = ITraitAccessor.TraitToVectorDirection(trait);
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
            var screenPos = _camera.WorldToScreenPoint(_worldPos + _traitDirection * labelBufferFromBoard);
            _rt.position = screenPos;
        }

        private void RepositionLabel(EventArgs obj = null) {
            var edgeHexes = traitAccessor.GetTraitEdgeHexes(trait);
            var direction = ITraitAccessor.TraitToVectorDirection(trait);
            var asVectors = edgeHexes.Select(h => controller.HexToWorldPos(h)).ToArray();
            var maxMag = asVectors.Max(v => v.magnitude);
            var maxVec = asVectors.First(v => Math.Abs(v.magnitude - maxMag) < 0.1f);

            var maxPosProjected = Vector3.Project(maxVec, direction.normalized);

            _worldPos = maxPosProjected;
            // transform.position = worldPosition;
        }
    }
}