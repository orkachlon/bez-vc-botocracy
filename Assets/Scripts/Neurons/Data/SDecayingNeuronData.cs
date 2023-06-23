using System.Collections.Generic;
using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "DecayNeuronData", menuName = "Neurons/Decay Neuron Data")]
    public class SDecayingNeuronData : SNeuronDataBase {

        [Header("Decay Neuron"), SerializeField] private int turnsToDeath;
        [SerializeField] private List<AudioClip> decayAddSounds;
        [SerializeField] private List<AudioClip> decayRemoveSounds;
        public int TurnsToDeath => turnsToDeath;
        public override IBoardNeuron GetNewElement() => NeuronFactory.GetBoardNeuron(Type);

        public AudioClip GetDecayAddSound() => decayAddSounds[Random.Range(0, decayAddSounds.Count)];
        public AudioClip GetDecayRemoveSound() => decayRemoveSounds[Random.Range(0, decayRemoveSounds.Count)];
    }
}