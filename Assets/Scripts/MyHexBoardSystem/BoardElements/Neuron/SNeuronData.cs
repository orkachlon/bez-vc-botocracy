using System;
using ExternBoardSystem.BoardElements;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron> {
        [SerializeField] private Neurons.Neuron.ENeuronType neuronType;
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardNeuron GetElement() {
            return new(this);
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public GameObject GetModel() {
            return model;
        }

        public Action<IBoardElementsController<BoardNeuron>, Vector3Int> GetActivation() {
            return neuronType switch {
                Neurons.Neuron.ENeuronType.Expanding => ExpandNeuron,
                Neurons.Neuron.ENeuronType.Exploding => ExplodeNeuron,
                _ => (_, _) => { }
            };
        }

        private static void ExpandNeuron(IBoardElementsController<BoardNeuron> elementsController, Vector3Int cell) {
            var neighbours = elementsController.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                if (elementsController.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                var newElement =
                    new BoardNeuron(MNeuronBindings.DataFromType(Neurons.Neuron.ENeuronType.Undefined));
                elementsController.AddElement(newElement, neighbour);
                MonoBehaviour.print($"Expanded to {neighbour}");
            }
        }

        private static void ExplodeNeuron(IBoardElementsController<BoardNeuron> elementsController, Vector3Int cell) {
            var neighbours = elementsController.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                var neighbourPos = elementsController.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || Neurons.Neuron.ENeuronType.Invulnerable.Equals(neighbourPos.Data.ElementData.neuronType))
                    continue;
                // explode this neuron
                elementsController.RemoveElement(neighbour);
                MonoBehaviour.print($"Exploded {neighbour}");
            }
        }
        
        
    }
}