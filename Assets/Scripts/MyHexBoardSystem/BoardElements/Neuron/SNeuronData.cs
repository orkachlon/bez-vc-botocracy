using System;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron> {
        [SerializeField] private Neurons.Neuron.ENeuronType neuronType;
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardNeuron GetElement() {
            return new BoardNeuron(this);
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public GameObject GetModel() {
            return model;
        }

        public Action<IBoardElementsController<BoardNeuron>, Vector3Int> GetActivation() {
            return neuronType switch {
                Neurons.Neuron.ENeuronType.Expanding => (elementsController, cell) => {
                    var neighbours = elementsController.Manipulator.GetNeighbours(cell);
                    foreach (var neighbour in neighbours) {
                        if (elementsController.Board.GetPosition(neighbour).HasData())
                            continue;
                        // expand to this hex
                        elementsController.AddElement(GetElement(), neighbour);
                        MonoBehaviour.print($"Expanded to {neighbour}");
                    }
                },
                Neurons.Neuron.ENeuronType.Exploding => (elementsController, cell) => {
                    var neighbours = elementsController.Manipulator.GetNeighbours(cell);
                    foreach (var neighbour in neighbours) {
                        if (elementsController.Board.GetPosition(neighbour).HasData()) {
                            // explode this neuron
                            elementsController.RemoveElement(neighbour);
                            MonoBehaviour.print($"Exploded {neighbour}");
                        }
                    }
                },
                _ => (_, _) => { }
            };
        }
    }
}