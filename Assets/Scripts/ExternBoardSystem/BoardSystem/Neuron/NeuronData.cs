using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Neuron {
    [CreateAssetMenu]
    public class NeuronData : ScriptableObject, IElementDataProvider {
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardElement GetElement() {
            return new BoardNeuron(this);
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public GameObject GetModel() {
            return model;
        }
    }
}