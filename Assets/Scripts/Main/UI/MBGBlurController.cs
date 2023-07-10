using System;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI {
    public class MBGBlurController : MonoBehaviour {

        private Image _blurImage;
        private Material _blurMaterial;

        private void Awake() {
            _blurImage = GetComponent<Image>();
            _blurMaterial = _blurImage.material;
        }
        
        public void SetAmount(float blurAmount) {
            _blurMaterial.SetFloat("_Radius", blurAmount);
        }

        public void SetColor(Color color) {
            _blurImage.color = color;
        }
    }
}