using System;
using ExternBoardSystem.Tools.Singleton;
using UnityEngine;

namespace Neurons {
    public class MNeuronTypeToPrefab : MSingleton<MNeuronTypeToPrefab> {
        // [Header("Neuron Prefabs")]
        // [SerializeField] private Neuron dummy;
        // [SerializeField] private Neuron invulnerable;
        // [SerializeField] private Neuron expand;
        // [SerializeField] private Neuron explode;
        //
        // public static MUINeuron GetNeuronPrefab(Neuron.ENeuronType type) {
        //     return type switch {
        //         Neuron.ENeuronType.Undefined => Instance.dummy,
        //         Neuron.ENeuronType.Invulnerable => Instance.invulnerable,
        //         Neuron.ENeuronType.Exploding => Instance.explode,
        //         Neuron.ENeuronType.Expanding => Instance.expand,
        //         _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        //     };
        // }
    }
}