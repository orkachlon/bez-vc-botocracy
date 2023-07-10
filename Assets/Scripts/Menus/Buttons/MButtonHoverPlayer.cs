using UnityEngine;
using UnityEngine.EventSystems;

namespace Menus.Buttons {
    [RequireComponent(typeof(AudioSource))]
    public class MButtonHoverPlayer : MonoBehaviour, IPointerEnterHandler {

        [SerializeField] private AudioClip hoverSound;
        [SerializeField, Range(0, 0.2f)] private float pitchVariation;
        
        private AudioSource _as;

        private void Awake() {
            _as = GetComponent<AudioSource>();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _as.pitch += (Random.value - 0.5f) * pitchVariation;
            _as.PlayOneShot(hoverSound);
        }
    }
}