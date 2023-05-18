using System;
using ExternBoardSystem.Tools.Singleton;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    public class MNeuronBindings : MSingleton<MNeuronBindings> {

        [SerializeField] private SNeuronData dummy;
        [SerializeField] private SNeuronData invulnerable;
        [SerializeField] private SNeuronData expand;
        [SerializeField] private SNeuronData explode;

        public static SNeuronData DataFromType(Neurons.Neuron.ENeuronType type) {
            switch (type) {
                case Neurons.Neuron.ENeuronType.Undefined:
                    return Instance.dummy;
                case Neurons.Neuron.ENeuronType.Invulnerable:
                    return Instance.invulnerable;
                case Neurons.Neuron.ENeuronType.Exploding:
                    return Instance.explode;
                case Neurons.Neuron.ENeuronType.Expanding:
                    return Instance.expand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}