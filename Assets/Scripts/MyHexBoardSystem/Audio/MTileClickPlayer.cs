using System;
using System.Collections.Generic;
using Core.EventSystem;
using MyHexBoardSystem.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyHexBoardSystem.Audio {
    public class MTileClickPlayer : MonoBehaviour {
        [Header("Clips"), SerializeField] private List<AudioClip> audioClips;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private AudioSource _source;

        private void Awake() {
            _source = GetComponent<AudioSource>();
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, PlayClick);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnAddTile, PlayClick);
        }

        private void PlayClick(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs) {
                return;
            }
            _source.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)]);
        }
    }
}