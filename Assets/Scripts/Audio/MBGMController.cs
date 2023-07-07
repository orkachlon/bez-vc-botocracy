using System;
using System.Collections;
using Core.EventSystem;
using Core.Utils.Singleton;
using DG.Tweening;
using Events.General;
using Types.GameState;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio {
    public class MBGMController : MSingleton<MBGMController> {

        [SerializeField] private AudioClip[] songs;
        
        [Header("Animation"), SerializeField] private float losePitchDuration;

        [Header("Event Managers"), SerializeField]
        private SEventManager gmEventManager;


        private AudioSource _as;
        private int _currentSong;
        private Tween _pitchAnimation;

        protected override void OnAwake() {
            _as = GetComponent<AudioSource>();
        }

        private void Start() {
            _currentSong = 0;
            _as.clip = songs[_currentSong];
            StartCoroutine(PlayOnRepeat());
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += HandleNewScene;
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, HandleGameState, true);
        }

        private void OnDisable() {
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, HandleGameState, true);
        }

        private void HandleGameState(EventArgs obj) {
            if (obj is not GameStateEventArgs { State:EGameState.Lose }) {
                return;
            }
            
            var pitchTween = SetPitch(0, losePitchDuration);
            pitchTween.OnComplete(Stop);
        }

        private void HandleNewScene(Scene arg0, LoadSceneMode arg1) {
            SetPitch(1, 0);
            if (!_as.isPlaying) {
                Play();
            }
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

        public static Tween SetPitch(float pitch, float duration = 0) {
            Instance._pitchAnimation?.Complete();
            Instance._pitchAnimation?.Kill();
            Instance._pitchAnimation = DOVirtual.Float(Instance._as.pitch, pitch, duration, p => Instance._as.pitch = p)
                .SetEase(Ease.InQuart)
                .OnComplete(() => Instance._pitchAnimation = null);
            return Instance._pitchAnimation;
        }
    }
}