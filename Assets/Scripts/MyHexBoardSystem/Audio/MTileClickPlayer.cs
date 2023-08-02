using System;
using System.Collections.Generic;
using Audio;
using Core.EventSystem;
using Events.Board;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyHexBoardSystem.Audio {
    public class MTileClickPlayer : MonoBehaviour {
        [Header("Clips"), SerializeField] private List<AudioClip> audioClips;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, PlayClick);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, PlayClick);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnAddTile, PlayClick);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveTile, PlayClick);
        }

        private void PlayClick(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs tileModifyArgs) {
                return;
            }
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = tileModifyArgs.Volume;
            s.Source.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)]);
            AudioSpawner.ReleaseWhenDone(s);
        }
    }
}