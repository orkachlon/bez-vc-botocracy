using Types.Audio;
using UnityEngine;

namespace Audio {
    [RequireComponent(typeof(AudioSource))]
    public class MGenericAudioSource : MonoBehaviour, IPoolableAudioSource {
        public AudioSource Source { get; private set; }
        public GameObject GO => gameObject;
        public void Default() { }
        
        private void Awake() {
            Source = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip clip) {
            Source.PlayOneShot(clip);
        }
    }
}