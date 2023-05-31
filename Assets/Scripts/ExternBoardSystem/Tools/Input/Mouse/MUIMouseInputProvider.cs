using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExternBoardSystem.Tools.Input.Mouse {
    [RequireComponent(typeof(Collider2D))]
    public partial class MUIMouseInputProvider : MonoBehaviour, IMouseInput {
        private Vector3 _prevPosition;
        private bool _isInsideBoard;
        public DragDirection Direction => GetDragDirection();
        public Vector2 MousePosition => UnityEngine.Input.mousePosition;
        public bool IsTracking { get; private set; }
        
        public void StartTracking() {
            IsTracking = true;
        }

        public void StopTracking() {
            IsTracking = false;
        }

        private void Awake() {
            // Can be used with PhysicsRaycaster2D and Collider2D too.
            if (Camera.main.GetComponent<Physics2DRaycaster>() == null)
                throw new Exception(GetType() + " needs a " + typeof(Physics2DRaycaster) + " on the MainCamera");
        }

        private void Update() {
            if (_isInsideBoard) {
                OnPointerStay?.Invoke(MousePosition);
            }
        }

        private DragDirection GetDragDirection() {
            var currentPosition = UnityEngine.Input.mousePosition;
            var normalized = (currentPosition - _prevPosition).normalized;
            _prevPosition = currentPosition;

            if (normalized.x > 0)
                return DragDirection.Right;

            if (normalized.x < 0)
                return DragDirection.Left;

            if (normalized.y > 0)
                return DragDirection.Top;

            return normalized.y < 0 ? DragDirection.Down : DragDirection.None;
        }
    }
}