using System.Collections.Generic;
using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "TravelNeuronData", menuName = "Neurons/Travel Neuron")]
    public class STravelNeuronData : SNeuronDataBase {
        
        [Header("Travel Neuron"), SerializeField] private int turnsToStop;
        [SerializeField] private AudioClip travelAddSound;
        [SerializeField] private List<AudioClip> travelMoveSounds;
        public int TurnsToStop => turnsToStop;
        
        public override IBoardNeuron GetNewElement()  => NeuronFactory.GetBoardNeuron(Type);

        public AudioClip GetTravelMoveSound() => travelMoveSounds[Random.Range(0, travelMoveSounds.Count)];
        public AudioClip GetTravelAddSound() => travelAddSound;
    }
}