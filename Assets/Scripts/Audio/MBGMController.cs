using System.Collections;
using Core.Utils.Singleton;
using UnityEngine;

namespace Audio {
    public class MBGMController : MSingleton<MBGMController> {

        [SerializeField] private AudioClip[] songs;


        private AudioSource _as;
        private int _currentSong;

        protected override void OnAwake() {
            _as = GetComponent<AudioSource>();
        }

        private void Start() {
            _currentSong = 0;
            _as.clip = songs[_currentSong];
            StartCoroutine(PlayOnRepeat());
        }

        private static IEnumerator PlayOnRepeat() {
            while (true) {
                Instance._as.Play();
                var waitUntilSongEnd = new WaitUntil(() => Instance._as.clip.length - Instance._as.time <= 0);
                yield return waitUntilSongEnd;
                Instance._currentSong = (Instance._currentSong + 1) % Instance.songs.Length;
                Instance._as.clip = Instance.songs[Instance._currentSong];
            }
        }

        public static void Play() {
            Instance._as.Play();
        }

        public static void Pause() {
            Instance._as.Pause();
        }

        public static void Stop() {
            Instance._as.Stop();
        }
    }
}