using UnityEngine;

namespace ExternBoardSystem.Ui.Particles {
    public class MUIPrimitiveHoverEffect : MonoBehaviour, IHoverEffect {

        private GameObject _hoverEffect;

        private void Awake() {
            _hoverEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        public void Show(Vector3 position) {
            _hoverEffect.SetActive(true);
            _hoverEffect.transform.position = position;
        }

        public void Hide() {
            _hoverEffect.SetActive(false);
        }
    }
}