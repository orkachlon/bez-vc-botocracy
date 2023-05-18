using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils {
    public class Unity : MonoBehaviour {

        private Camera _mainCam;

        private static Unity _instance;

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
    public static class EnumUtil {
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T GetRandom<T>() {
            var allValues = GetValues<T>().ToArray();
            var rndSelection = Random.Range(0, allValues.Length);
            return allValues[rndSelection];
        }
    }
}
