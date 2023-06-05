﻿using System;
using UnityEngine;

namespace Main.Controls {

    public class MDragController : MonoBehaviour {
        
        private Vector2 _mouseClickPos;
        private Vector2 _mouseCurrentPos;
        
        private Camera _camera;

        private void Awake() {
            _camera = GetComponentInChildren<Camera>();
        }

        private void LateUpdate() {
            
            // When LMB clicked get mouse click position and set panning to true
            if (Input.GetKey(KeyCode.Mouse2))
            {
                if (_mouseClickPos == default)
                {
                    _mouseClickPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                }
 
                _mouseCurrentPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                var distance = _mouseCurrentPos - _mouseClickPos;
                transform.position += new Vector3(-distance.x, -distance.y, 0);
            }
 
            // If LMB is released, stop moving the camera
            if (Input.GetKeyUp(KeyCode.Mouse2))
                _mouseClickPos = default;
        }
    }
}