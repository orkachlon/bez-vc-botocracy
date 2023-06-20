using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "DecayNeuronData", menuName = "Neurons/Decay Neuron Data")]
    public class SDecayingNeuronData : SNeuronDataBase {

        [Header("Decay Neuron"), SerializeField] private int turnsToDeath;
        public int TurnsToDeath => turnsToDeath;
        public override IBoardNeuron GetNewElement() => NeuronFactory.GetBoardNeuron(Type);
    }
}