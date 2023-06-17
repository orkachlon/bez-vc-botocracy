using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "InvlunerableNeuronData", menuName = "Neurons/Invulnerable Neuron")]
    public class SInvulnerableNeuronData : SNeuronDataBase {
        public override IBoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }
    }
}