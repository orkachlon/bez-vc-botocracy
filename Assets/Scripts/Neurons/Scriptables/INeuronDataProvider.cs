using MyHexBoardSystem.BoardSystem.Elements;
using UnityEngine;

namespace Neurons.Scriptables {
    public interface INeuronDataProvider : IElementDataProvider {
        Neuron.NeuronType GetNeuronType();

        Sprite GetArtwork();
    }
}