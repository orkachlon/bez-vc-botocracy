using UnityEngine;

namespace Main.Controls {
    
    
    [RequireComponent(typeof(Camera))]
    public class MCameraZoomController : MonoBehaviour {

        [SerializeField] private float maxZoomIn, maxZoomOut;
        [SerializeField] private float zoomSpeed;

        private Camera _camera;
        
        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        private void Update() {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _camera.orthographicSize < maxZoomOut) { // zoom out
                _camera.orthographicSize += zoomSpeed * Time.deltaTime;
            } else if (Input.GetAxis("Mouse ScrollWheel") > 0 && _camera.orthographicSize > maxZoomIn) { // zoom in
                _camera.orthographicSize -= zoomSpeed * Time.deltaTime;
            }
        }
    }
}