using System;
using UnityEngine;

namespace Utils {
    public class DestroyOnLoad : MonoBehaviour {
        private void Start() {
            Destroy(gameObject);
        }
    }
}