using UnityEngine;

namespace Core.Utils {
    public class MLogger : MonoBehaviour {
        public static void Log(string message) {
            print(message);
        }
        
        public static void LogEditor(string message) {
#if UNITY_EDITOR
            print(message);
#endif
        }

        public static void LogEditorError(string message) {
            Debug.LogError(message);
        }

        public static void LogEditorWarning(string message) {
            Debug.LogWarning(message);
        }
    }
}