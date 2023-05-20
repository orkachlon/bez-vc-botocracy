﻿using System;
using ExternBoardSystem.BoardElements;
using MyHexBoardSystem.UI;
using Neurons;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu(menuName = "Neurons/Neuron Data")]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron, MUIBoardNeuron> {
        [SerializeField] private ENeuronType neuronType;

        public ENeuronType Type {
            get => neuronType;
            set => neuronType = value;
        }
        
        [Header("Sprite")]
        [SerializeField] private Sprite artwork;
        
        [Header("Pooling Model")]
        [SerializeField] private MUIBoardNeuron model;

        public void SetData(SNeuronData other) {
            Type = other.Type;
            artwork = other.artwork;
            model = other.model;
        }

        public BoardNeuron GetElement() {
            return new BoardNeuron(this);
        }

        public MUIBoardNeuron GetModel() {
            return model;
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public Action<IBoardElementsController<BoardNeuron>, Vector3Int> GetActivation() {
            return Type switch {
                ENeuronType.Expanding => ExpandNeuron,
                ENeuronType.Exploding => ExplodeNeuron,
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
                    new BoardNeuron(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy));
                elementsController.AddElement(newElement, neighbour);
            }
        }

        private static void ExplodeNeuron(IBoardElementsController<BoardNeuron> elementsController, Vector3Int cell) {
            var neighbours = elementsController.Manipulator.GetNeighbours(cell);
            foreach (var neighbour in neighbours) {
                var neighbourPos = elementsController.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || ENeuronType.Invulnerable.Equals(neighbourPos.Data.DataProvider.Type))
                    continue;
                // explode this neuron
                elementsController.RemoveElement(neighbour);
            }
        }

        public override string ToString() {
            return $"{neuronType}";
        }
    }
}