using UnityEngine;

namespace Main.Neurons.Data {
    [CreateAssetMenu(fileName = "DecayNeuronData", menuName = "Neurons/Decay Neuron Data")]
    public class SDecayingNeuronData : SNeuronDataBase {

        [Header("Decay Neuron"), SerializeField] private int turnsToDeath;
        public int TurnsToDeath => turnsToDeath;
    }
}