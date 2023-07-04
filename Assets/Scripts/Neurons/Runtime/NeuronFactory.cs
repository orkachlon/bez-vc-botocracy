using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Connections;
using Types.Neuron;
using Random = UnityEngine.Random;

namespace Neurons.Runtime {
    public static class NeuronFactory {

        public static readonly ENeuronType[] PlaceableNeurons = new[] {
            ENeuronType.Decaying,
            ENeuronType.Expanding,
            ENeuronType.Exploding,
            ENeuronType.Travelling
        };
        
        public static BoardNeuron GetBoardNeuron(ENeuronType neuronType) {
            return neuronType switch {
                ENeuronType.Undefined => null,
                ENeuronType.Invulnerable => new InvulnerableNeuron(),
                ENeuronType.Exploding => new ExplodeNeuron(),
                ENeuronType.Expanding => new ExpandNeuron(),
                ENeuronType.Dummy => new DummyNeuron(),
                ENeuronType.Decaying => new DecayNeuron(),
                ENeuronType.Travelling => new TravelNeuron(),
                _ => throw new ArgumentOutOfRangeException(nameof(neuronType), neuronType, null)
            };
        }
        
        public static BoardNeuron GetRandomNeuron(IEnumerable<ENeuronType> fromTypes = null) {
            fromTypes ??= EnumUtil.GetValues<ENeuronType>();
            var asArray = fromTypes
                .Where(t => t != ENeuronType.Undefined)
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            return GetBoardNeuron(rnd);
        }
        
        public static BoardNeuron GetRandomPlaceableNeuron() {
            var asArray = EnumUtil.GetValues<ENeuronType>()
                .Where(t => PlaceableNeurons.Contains(t))
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            return GetBoardNeuron(rnd);
        }

        public static BoardNeuronConnector GetConnector() {
            return new(MConnectionManager.Instance);
        }
    }
}