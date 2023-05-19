using System;
using ExternBoardSystem.BoardElements;
using MyHexBoardSystem.UI;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron, MUIBoardNeuron> {
        [SerializeField] private Neurons.Neuron.ENeuronType neuronType;
        
        [Header("Visuals")]
        [SerializeField] private Sprite artwork;
        
        // todo think about using only 1 model
        [Header("Pooling Model")]
        [SerializeField] private MUIBoardNeuron model;

        public BoardNeuron GetElement() {
            return new(this);
        }

        public MUIBoardNeuron GetModel() {
            return model;
        }

        public Sprite GetArtwork() {
            return artwork;
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
                    new BoardNeuron(MNeuronTypeToBoardData.GetNeuronData(Neurons.Neuron.ENeuronType.Undefined));
                elementsController.AddElement(newElement, neighbour);
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
            }
        }
        
        
    }
}