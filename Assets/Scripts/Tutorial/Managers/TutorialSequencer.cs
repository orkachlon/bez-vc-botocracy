using System.Threading.Tasks;
using UnityEngine;

namespace Tutorial.Managers {
    public class TutorialSequencer : MonoBehaviour {
        private async void Start() {
            await TutorialSequence();
        }

        private async Task TutorialSequence() {
            // introduction - place a neuron
        }
    }
}