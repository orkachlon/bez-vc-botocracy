using System;
using ExternBoardSystem.Tools.Singleton;
using UnityEngine;

namespace Neurons {
    public class MNeuronTypeToPrefab : MSingleton<MNeuronTypeToPrefab> {
        [Header("Neuron Prefabs")]
        [SerializeField] private MNeuron dummy;
        [SerializeField] private MNeuron invulnerable;
        [SerializeField] private MNeuron expand;
        [SerializeField] private MNeuron explode;

        public static MNeuron GetNeuronPrefab(MNeuron.ENeuronType type) {
            switch (type) {
                case MNeuron.ENeuronType.Undefined:
                    return Instance.dummy;
                case MNeuron.ENeuronType.Invulnerable:
                    return Instance.invulnerable;
                case MNeuron.ENeuronType.Exploding:
                    return Instance.explode;
                case MNeuron.ENeuronType.Expanding:
                    return Instance.expand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}