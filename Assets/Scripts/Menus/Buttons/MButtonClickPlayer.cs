using Types.Menus;
using UnityEngine;

namespace Menus.Buttons {
    
    [RequireComponent(typeof(AudioSource))]
    public class MButtonClickPlayer : MonoBehaviour, IMenuButton {

        private AudioSource _as;

        private void Awake() {
            _as = GetComponent<AudioSource>();
        }

        public void OnButtonClick() {
            _as.pitch += (Random.value - 0.5f) * 0.25f;
            _as.Play();
        }
    }
}