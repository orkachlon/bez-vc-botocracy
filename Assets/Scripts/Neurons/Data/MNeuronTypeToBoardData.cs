using System;
using Core.Utils.Singleton;
using MyHexBoardSystem.BoardElements.Neuron.Data;
using Types.Neuron;
using UnityEngine;

namespace Neurons.Data {
    public class MNeuronTypeToBoardData : MSingleton<MNeuronTypeToBoardData> {

        [Header("Board Neuron Data")]
        [SerializeField] private SNeuronDataBase invulnerable;
        [SerializeField] private SNeuronDataBase dummy;
        [SerializeField] private SNeuronDataBase expand;
        [SerializeField] private SNeuronDataBase explode;
        [SerializeField] private SNeuronDataBase decay;
        [SerializeField] private SNeuronDataBase travel;

        public static SNeuronDataBase GetNeuronData(ENeuronType type) {
            return type switch {
                ENeuronType.Dummy => Instance.dummy,
                ENeuronType.Invulnerable => Instance.invulnerable,
                ENeuronType.Exploding => Instance.explode,
                ENeuronType.Expanding => Instance.expand,
                ENeuronType.Decaying => Instance.decay,
                ENeuronType.Travelling => Instance.travel,
                ENeuronType.Undefined => null,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}