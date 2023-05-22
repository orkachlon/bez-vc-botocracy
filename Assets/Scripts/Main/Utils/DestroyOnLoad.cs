using UnityEngine;

namespace Main.Utils {
    public class DestroyOnLoad : MonoBehaviour {
        private void Start() {
            Destroy(gameObject);
        }
    }
}