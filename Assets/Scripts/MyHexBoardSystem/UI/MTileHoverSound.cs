using System;
using Audio;
using Core.EventSystem;
using Events.Board;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MyHexBoardSystem.UI {
    public class MTileHoverSound : MonoBehaviour {
        
        [FormerlySerializedAs("hoverTileSound")] [SerializeField] private AudioClip hoverTileLegalSound;
        [SerializeField] private AudioClip hoverTileIllegalSound;
        [SerializeField, Range(0, 1)] private float volume;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;


        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTileHover, PlaySound);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTileHover, PlaySound);
        }

        private void PlaySound(EventArgs eventData) {
            if (eventData is not TileHoverArgs hoverArgs) {
                return;
            }
            
            Play(hoverArgs.LegalPlacement ? hoverTileLegalSound : hoverTileIllegalSound);
        }

        private void Play(AudioClip sound) {
            var s = AudioSpawner.GetAudioSource();
            s.Source.pitch += (Random.value - 0.5f) * 0.1f;
            s.Source.volume = volume;
            s.Source.PlayOneShot(sound);
            AudioSpawner.ReleaseWhenDone(s);
        }
    }
}