using System.Collections;
using Core.Tools.Pooling;
using Core.Utils;
using Types.Audio;
using UnityEngine;

namespace Audio {
    public class AudioSpawner : MonoBehaviour {

        [Header("Defaults"), SerializeField] private MGenericAudioSource defaultAudioSourcePrefab;

        public static AudioSpawner Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        public static void PoolSound(AudioClip clip) {
            var source = MObjectPooler.Instance.GetPoolable(Instance.defaultAudioSourcePrefab);
            if (source == null) {
                MLogger.LogEditor("Failed to pool generic audio source");
                return;
            }
            source.Source.PlayOneShot(clip);
            Instance.StartCoroutine(ReleaseWhenDone(source));
        }

        public static IAudioSource GetAudioSource() {
            return Instantiate(Instance.defaultAudioSourcePrefab);
        }

        private static IEnumerator ReleaseWhenDone(MGenericAudioSource pooledAudioSource) {
            yield return new WaitWhile(() => pooledAudioSource.Source.isPlaying);
            MObjectPooler.Instance.Release(pooledAudioSource.GO);
        }
    }
}