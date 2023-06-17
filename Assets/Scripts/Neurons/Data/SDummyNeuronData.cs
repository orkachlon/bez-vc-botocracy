using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "DummyNeuronData", menuName = "Neurons/Dummy Neuron")]
    public class SDummyNeuronData : SNeuronDataBase {
        public override IBoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }
    }
}