using System;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.BoardElements.Neuron.Data {
    public class SNeuronDataBase : ScriptableObject, INeuronDataBase {
        [SerializeField] private ENeuronType neuronType;

        public ENeuronType Type {
            get => neuronType;
            private set => neuronType = value;
        }

        public IBoardNeuron RuntimeElement => GetNewElement();

        public Color ConnectionColor => connectionColor;

        [Header("Sprites"), SerializeField] private Sprite boardArtwork;
        [SerializeField] private Sprite UIStackArtwork;
        [SerializeField] private Sprite UITopArtwork;
        [SerializeField] private Sprite faceSprite;
        [SerializeField] private Color connectionColor;
        
        [Header("Prefab  Models"), SerializeField] private MUIBoardNeuron boardModel;
        [SerializeField] private MUIQueueNeuron UIModel;

        [Header("Sounds"), SerializeField] private AudioClip addSound;
        [SerializeField] private AudioClip removeSound;

        [Header("Effect Markers"), SerializeField] protected TileBase effectTile;

        public void SetData(INeuronDataBase other) {
            Type = other.Type;
            boardArtwork = other.GetBoardArtwork();
            boardModel = other.GetModel() as MUIBoardNeuron;
        }

        public virtual IBoardNeuron GetNewElement() => null;
        public virtual IUIBoardNeuron GetModel() => boardModel;
        public IUIQueueNeuron GetUIModel() => UIModel;
        public Sprite GetBoardArtwork() => boardArtwork;
        public Sprite GetFaceSprite() => faceSprite;
        public TileBase GetEffectTile() => effectTile;

        public AudioClip GetAddSound() => addSound;
        public AudioClip GetRemoveSound() => removeSound;
        public Sprite GetQueueStackArtwork() => UIStackArtwork;
        public Sprite GetQueueTopArtwork() => UITopArtwork;


        public override string ToString() => $"{neuronType}";
    }
}