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

        public static SNeuronData GetNeuronData(Neurons.Neuron.ENeuronType type) {
            return type switch {
                Neurons.Neuron.ENeuronType.Dummy => Instance.dummy,
                Neurons.Neuron.ENeuronType.Invulnerable => Instance.invulnerable,
                Neurons.Neuron.ENeuronType.Exploding => Instance.explode,
                Neurons.Neuron.ENeuronType.Expanding => Instance.expand,
                Neurons.Neuron.ENeuronType.Undefined => null,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}