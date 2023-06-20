using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "ExpandNeuronData", menuName = "Neurons/Expand Neuron Data")]
    public class SExpandNeuronData : SNeuronDataBase {
        public override IBoardNeuron GetNewElement()  => NeuronFactory.GetBoardNeuron(Type);
    }
}