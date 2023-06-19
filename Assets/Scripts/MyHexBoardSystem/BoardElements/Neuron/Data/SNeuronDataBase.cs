﻿using System;
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
            private set => neuronType = value;
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

        [Header("Sounds"), SerializeField] private AudioClip addSound;

        public void SetData(INeuronDataBase other) {
            Type = other.Type;
            boardArtwork = other.GetBoardArtwork();
            boardModel = other.GetModel() as MUIBoardNeuron;
        }

        public virtual IBoardNeuron GetElement() => null;
        public virtual IUIBoardNeuron GetModel() => boardModel;
        public IUINeuron GetUIModel() => UIModel;
        public Sprite GetBoardArtwork() => boardArtwork;
        public AudioClip GetAddSound() => addSound;
        public Sprite GetUIArtwork(ENeuronUIState uiState) => uiState switch {
            ENeuronUIState.Stack => UIStackArtwork,
            ENeuronUIState.Third => UIThirdArtwork,
            ENeuronUIState.Second => UISecondArtwork,
            ENeuronUIState.First => UIFirstArtwork,
            _ => throw new ArgumentOutOfRangeException(nameof(uiState), uiState, null)
        };
        
        public override string ToString() => $"{neuronType}";
    }
}