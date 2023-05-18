using System;
using ExternBoardSystem.BoardElements;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron> {
        [SerializeField] private Neurons.MNeuron.ENeuronType neuronType;
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
                Neurons.MNeuron.ENeuronType.Expanding => ExpandNeuron,
                Neurons.MNeuron.ENeuronType.Exploding => ExplodeNeuron,
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
                    new BoardNeuron(MNeuronTypeToBoardData.GetNeuronData(Neurons.MNeuron.ENeuronType.Undefined));
                elementsController.AddElement(newElement, neighbour);
            }
        }

        private static void ExplodeNeuron(IBoardElementsController<BoardNeuron> elementsController, Vector3Int cell) {
            var neighbours = elementsController.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                var neighbourPos = elementsController.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || Neurons.MNeuron.ENeuronType.Invulnerable.Equals(neighbourPos.Data.ElementData.neuronType))
                    continue;
                // explode this neuron
                elementsController.RemoveElement(neighbour);
            }
        }
        
        
    }
}