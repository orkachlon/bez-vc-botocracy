using UnityEngine;

namespace Main.Neurons.Data {
    [CreateAssetMenu(fileName = "TravelNeuronData", menuName = "Neurons/Travel Neuron")]
    public class STravelNeuronData : SNeuronDataBase {
        
        [Header("Travel Neuron"), SerializeField] private int turnsToStop;
        public int TurnsToStop => turnsToStop;
    }
}