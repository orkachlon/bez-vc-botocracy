using System;
using Core.EventSystem;
using Events.UI;
using UnityEngine;

namespace Main.Controls {

    public class MDragController : MonoBehaviour {
        
        [SerializeField] private SEventManager uiEventManager;
        
        private Vector2 _mouseClickPos;
        private Vector2 _mouseCurrentPos;
        
        private Camera _camera;

        private bool _enabled = true;
        
        
        #region UnityMethods

        private void Awake() {
            _camera = GetComponentInChildren<Camera>();
        }
        
        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnOverlayShow, DisableDrag);
            uiEventManager.Register(UIEvents.OnOverlayHide, EnableDrag);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnOverlayShow, DisableDrag);
            uiEventManager.Unregister(UIEvents.OnOverlayHide, EnableDrag);
        }

        private void LateUpdate() {
            if (!_enabled) {
                return;
            }
            // When RMB clicked get mouse click position and set panning to true
            if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2)) {
                if (_mouseClickPos == default) {
                    _mouseClickPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                }
 
                _mouseCurrentPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                var distance = _mouseCurrentPos - _mouseClickPos;
                transform.position += new Vector3(-distance.x, -distance.y, 0);
            }
 
            // If RMB is released, stop moving the camera
            if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2)) {
                _mouseClickPos = default;
            }
        }

        #endregion

        #region EventHandlers

        private void EnableDrag(EventArgs obj) {
            _enabled = true;
        }

        private void DisableDrag(EventArgs obj) {
            _enabled = false;
        }

        #endregion
    }
}