using Audio;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using UnityEngine;

namespace Neurons.UI {
    public class MUIDummyNeuron : MUIBoardNeuron {

        #region Sound

        public override void PlayRemoveSound() {
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = removeVolume;
            s.Source.pitch += (Random.value - 0.5f) * 0.5f;
            s.Source.PlayOneShot(RuntimeData.DataProvider.GetRemoveSound());
            AudioSpawner.ReleaseWhenDone(s);
        }

        #endregion
        
    }
}