using UnityEngine;

namespace Utils {
    public class Utils : MonoBehaviour {

        private Camera _mainCam;

        private static Utils _instance;

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this);
            }
            else {
                _instance = this;
            }
            _mainCam = Camera.main;
        }

        public static Vector3 GetMousePos(float z = 0) {
            var projectedMousePos = _instance._mainCam.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(projectedMousePos.x, projectedMousePos.y, z);
        }

        public static void SetCursorVisibility(bool isVisible) {
            Cursor.visible = isVisible;
        }
    }
}
