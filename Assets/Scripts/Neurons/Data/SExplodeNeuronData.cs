using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "ExplodeNeuronData", menuName = "Neurons/Explode Neuron Data")]
    public class SExplodeNeuronData : SNeuronDataBase {

        [SerializeField] private AudioClip killSound;
        
        public override IBoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }

        public AudioClip GetKillSound() => killSound;
    }
}