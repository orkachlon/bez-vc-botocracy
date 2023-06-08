using UnityEngine;

namespace Main.Controls {
    
    
    // DOESN'T WORK YET
    [RequireComponent(typeof(Camera))]
    public class MCameraZoomController : MonoBehaviour {

        [SerializeField] private float maxZoom, minZoom;
        [SerializeField] private float zoomSpeed;

        private Camera _camera;
        
        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        private void Update() {
            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                _camera.sensorSize += Vector2.one * (zoomSpeed * Time.deltaTime);
            } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                _camera.sensorSize -= Vector2.one * (zoomSpeed * Time.deltaTime);
            }
        }
    }
}