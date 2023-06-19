using Types.Audio;
using Types.Pooling;
using UnityEngine;

namespace Audio {
    [RequireComponent(typeof(AudioSource))]
    public class MGenericAudioSource : MonoBehaviour, IAudioSource, IPoolable {
        public AudioSource Source { get; private set; }
        public GameObject GO => gameObject;


        private void Awake() {
            Source = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip) {
            Source.PlayOneShot(clip);
        }
    }
}