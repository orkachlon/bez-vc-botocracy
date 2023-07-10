using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "DummyNeuronData", menuName = "Neurons/Dummy Neuron")]
    public class SDummyNeuronData : SNeuronDataBase {

        public override Color ConnectionColor => base.ConnectionColor;

        public override void SetData(INeuronDataBase other) {
            base.SetData(other);
        }


        public override IBoardNeuron GetNewElement() => NeuronFactory.GetBoardNeuron(Type);
    }
}