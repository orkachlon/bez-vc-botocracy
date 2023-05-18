using System;
using ExternBoardSystem.Tools.Singleton;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    public class MNeuronTypeToBoardData : MSingleton<MNeuronTypeToBoardData> {

        [Header("Board Neuron Data")]
        [SerializeField] private SNeuronData dummy;
        [SerializeField] private SNeuronData invulnerable;
        [SerializeField] private SNeuronData expand;
        [SerializeField] private SNeuronData explode;

        public static SNeuronData GetNeuronData(Neurons.MNeuron.ENeuronType type) {
            switch (type) {
                case Neurons.MNeuron.ENeuronType.Undefined:
                    return Instance.dummy;
                case Neurons.MNeuron.ENeuronType.Invulnerable:
                    return Instance.invulnerable;
                case Neurons.MNeuron.ENeuronType.Exploding:
                    return Instance.explode;
                case Neurons.MNeuron.ENeuronType.Expanding:
                    return Instance.expand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}