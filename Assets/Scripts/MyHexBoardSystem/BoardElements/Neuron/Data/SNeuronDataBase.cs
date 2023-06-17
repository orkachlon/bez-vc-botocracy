using System;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements.Neuron.Data {
    public class SNeuronDataBase : ScriptableObject, INeuronDataBase {
        [SerializeField] private ENeuronType neuronType;

        public ENeuronType Type {
            get => neuronType;
            set => neuronType = value;
        }

        public IBoardNeuron RuntimeElement => GetElement();

        public Color ConnectionColor => connectionColor;

        [Header("Sprites"), SerializeField] private Sprite boardArtwork;
        [SerializeField] private Sprite UIStackArtwork;
        [SerializeField] private Sprite UIThirdArtwork;
        [SerializeField] private Sprite UISecondArtwork;
        [SerializeField] private Sprite UIFirstArtwork;
        [SerializeField] private Color connectionColor;
        
        [Header("Prefab  Models"), SerializeField] private MUIBoardNeuron boardModel;
        [SerializeField] private MUINeuron UIModel;

        public void SetData(INeuronDataBase other) {
            Type = other.Type;
            boardArtwork = other.GetBoardArtwork();
            boardModel = other.GetModel() as MUIBoardNeuron;
        }

        public virtual IBoardNeuron GetElement() {
            return null;
        }

        public virtual IUIBoardNeuron GetModel() {
            return boardModel;
        }

        public IUINeuron GetUIModel() {
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