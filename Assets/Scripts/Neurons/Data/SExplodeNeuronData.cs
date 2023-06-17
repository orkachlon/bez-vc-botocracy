using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "ExplodeNeuronData", menuName = "Neurons/Explode Neuron Data")]
    public class SExplodeNeuronData : SNeuronDataBase {
        public override IBoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }
    }
}