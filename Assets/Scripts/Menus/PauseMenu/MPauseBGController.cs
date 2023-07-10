using UnityEngine;
using UnityEngine.UI;

namespace Menus.PauseMenu {
    public class MPauseBGController : MonoBehaviour {
        
        private Image _blurImage;

        private void Awake() {
            _blurImage = GetComponent<Image>();
        }

        public void SetColor(Color color) {
            _blurImage.color = color;
        }
    }
}