using System;
using Audio;
using Core.EventSystem;
using Events.Board;
using UnityEngine;

namespace Neurons.Audio {
    public class MNeuronFailedPlayer : MonoBehaviour {

        [SerializeField] private AudioClip failSound;
        
        [SerializeField] private SEventManager boardEventManager;

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementFailed, PlayFailedSound);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementFailed, PlayFailedSound);
        }

        private void PlayFailedSound(EventArgs obj) {
            AudioSpawner.PoolSound(failSound);
        }
    }
}