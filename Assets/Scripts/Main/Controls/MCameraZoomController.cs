using System;
using Core.EventSystem;
using Events.SP;
using Events.UI;
using UnityEngine;

namespace Main.Controls {
    
    
    [RequireComponent(typeof(Camera))]
    public class MCameraZoomController : MonoBehaviour {

        [SerializeField] private float maxZoomIn, maxZoomOut;
        [SerializeField] private float zoomSpeed;

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager uiEventManager;

        private Camera _camera;
        private bool _zoomEnabled = true;
        
        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnOutcomesEnter, DisableZoom);
            storyEventManager.Register(StoryEvents.OnOutcomesExit, EnableZoom);
            uiEventManager.Register(UIEvents.OnOverlayShow, DisableZoom);
            uiEventManager.Register(UIEvents.OnOverlayHide, EnableZoom);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnOutcomesEnter, DisableZoom);
            storyEventManager.Unregister(StoryEvents.OnOutcomesExit, EnableZoom);
            uiEventManager.Unregister(UIEvents.OnOverlayShow, DisableZoom);
            uiEventManager.Unregister(UIEvents.OnOverlayHide, EnableZoom);
        }

        private void Update() {
            if (!_zoomEnabled) {
                return;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _camera.orthographicSize < maxZoomOut) { // zoom out
                _camera.orthographicSize += zoomSpeed * Time.deltaTime;
            } else if (Input.GetAxis("Mouse ScrollWheel") > 0 && _camera.orthographicSize > maxZoomIn) { // zoom in
                _camera.orthographicSize -= zoomSpeed * Time.deltaTime;
            }
        }

        private void EnableZoom(EventArgs obj) {
            _zoomEnabled = true;
        }

        private void DisableZoom(EventArgs obj) {
            _zoomEnabled = false;
        }
    }
}