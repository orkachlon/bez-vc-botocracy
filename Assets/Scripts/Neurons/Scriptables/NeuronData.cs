using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;

namespace Neurons.Scriptables {
    [CreateAssetMenu(fileName = "NeuronData", menuName = "Neuron ElementData", order = 0)]
    public class NeuronData : ScriptableObject, INeuronDataProvider {

        [SerializeField] private Neuron.NeuronType type;
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;
        
        public Neuron.NeuronType GetNeuronType() {
            return type;
        }

        public BoardElement GetElement() {
            return new BoardNeuron(this);
        }

        public GameObject GetModel() {
            return model;
        }

        public Sprite GetArtwork() {
            return artwork;
        }
    }
}