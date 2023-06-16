using System;
using ExternBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Runtime;
using Main.Neurons.UI;
using UnityEngine;

namespace Main.Neurons.Data {
    public class SNeuronDataBase : ScriptableObject, IElementDataProvider<BoardNeuron, MUIBoardNeuron> {
        [SerializeField] private ENeuronType neuronType;

        public ENeuronType Type {
            get => neuronType;
            set => neuronType = value;
        }

        public Color ConnectionColor => connectionColor;

        [Header("Sprites"), SerializeField] private Sprite boardArtwork;
        [SerializeField] private Sprite UIStackArtwork;
        [SerializeField] private Sprite UIThirdArtwork;
        [SerializeField] private Sprite UISecondArtwork;
        [SerializeField] private Sprite UIFirstArtwork;
        [SerializeField] private Color connectionColor;
        
        [Header("Prefab  Models"), SerializeField] private MUIBoardNeuron boardModel;
        [SerializeField] private MUINeuron UIModel;

        public void SetData(SNeuronDataBase other) {
            Type = other.Type;
            boardArtwork = other.boardArtwork;
            boardModel = other.boardModel;
        }

        public virtual BoardNeuron GetElement() {
            return NeuronFactory.GetBoardNeuron(Type);
        }

        public virtual MUIBoardNeuron GetModel() {
            return boardModel;
        }

        public MUINeuron GetUIModel() {
            return UIModel;
        }

        public Sprite GetBoardArtwork() {
            return boardArtwork;
        }

        public Sprite GetUIArtwork(ENeuronUIState uiState) {
            return uiState switch {
                ENeuronUIState.Stack => UIStackArtwork,
                ENeuronUIState.Third => UIThirdArtwork,
                ENeuronUIState.Second => UISecondArtwork,
                ENeuronUIState.First => UIFirstArtwork,
                _ => throw new ArgumentOutOfRangeException(nameof(uiState), uiState, null)
            };
        }
        
        public override string ToString() {
            return $"{neuronType}";
        }
    }
}