using System;
using System.Linq;
using Main.Neurons.Runtime;
using Main.Utils;
using Random = UnityEngine.Random;

namespace Main.Neurons {
    public static class NeuronFactory {
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
        
        public static BoardNeuron GetRandomNeuron() {
            var asArray = EnumUtil.GetValues<ENeuronType>()
                .Where(t => t != ENeuronType.Undefined)
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            return NeuronFactory.GetBoardNeuron(rnd);
        }
        
        public static BoardNeuron GetRandomPlaceableNeuron() {
            var asArray = EnumUtil.GetValues<ENeuronType>()
                .Where(t => t != ENeuronType.Undefined && t != ENeuronType.Dummy && t != ENeuronType.Invulnerable)
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            return NeuronFactory.GetBoardNeuron(rnd);
        }
    }
}