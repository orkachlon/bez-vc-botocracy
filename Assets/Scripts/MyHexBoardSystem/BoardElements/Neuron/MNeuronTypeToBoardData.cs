using System;
using ExternBoardSystem.Tools.Singleton;
using Neurons;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    public class MNeuronTypeToBoardData : MSingleton<MNeuronTypeToBoardData> {

        [Header("Board Neuron Data")]
        [SerializeField] private SNeuronData dummy;
        [SerializeField] private SNeuronData invulnerable;
        [SerializeField] private SNeuronData expand;
        [SerializeField] private SNeuronData explode;

        public static SNeuronData GetNeuronData(ENeuronType type) {
            return type switch {
                ENeuronType.Dummy => Instance.dummy,
                ENeuronType.Invulnerable => Instance.invulnerable,
                ENeuronType.Exploding => Instance.explode,
                ENeuronType.Expanding => Instance.expand,
                ENeuronType.Undefined => null,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}