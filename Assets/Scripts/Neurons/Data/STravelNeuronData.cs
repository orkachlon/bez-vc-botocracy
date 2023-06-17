using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "TravelNeuronData", menuName = "Neurons/Travel Neuron")]
    public class STravelNeuronData : SNeuronDataBase {
        
        [Header("Travel Neuron"), SerializeField] private int turnsToStop;
        public int TurnsToStop => turnsToStop;
        
        public override IBoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }
    }
}