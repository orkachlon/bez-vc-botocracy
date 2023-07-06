using Types.Menus;
using UnityEngine;

namespace Menus.Buttons {
    
    [RequireComponent(typeof(AudioSource))]
    public class MWooshPlayer : MonoBehaviour, IMenuButton {

        [SerializeField] private AudioClip wooshSound;
        
        private AudioSource _as;

        private void Awake() {
            _as = GetComponent<AudioSource>();
        }

        public void OnButtonClick() {
            _as.PlayOneShot(wooshSound);
        }
    }
}