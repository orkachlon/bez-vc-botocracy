using System.Collections;
using Core.Tools.Pooling;
using Core.Utils;
using Core.Utils.Singleton;
using Types.Audio;
using UnityEngine;

namespace Audio {
    public class AudioSpawner : MSingleton<AudioSpawner> {

        [Header("Defaults"), SerializeField] private MGenericAudioSource defaultAudioSourcePrefab;

        public static void PoolSound(AudioClip clip) {
            var source = MObjectPooler.Instance.GetPoolable(Instance.defaultAudioSourcePrefab);
            if (source == null) {
                MLogger.LogEditor("Failed to pool generic audio source");
                return;
            }
            source.Source.PlayOneShot(clip);
            ReleaseWhenDone(source);
        }

        public static IPoolableAudioSource GetAudioSource() {
            return Instantiate(Instance.defaultAudioSourcePrefab);
        }

        public static void ReleaseWhenDone(IPoolableAudioSource source) {
            Instance.StartCoroutine(ReleaseWhenDoneHelper(source));
        }

        private static IEnumerator ReleaseWhenDoneHelper(IPoolableAudioSource pooledAudioSource) {
            yield return new WaitWhile(() => pooledAudioSource.Source.isPlaying);
            MObjectPooler.Instance.Release(pooledAudioSource.GO);
        }
    }
}