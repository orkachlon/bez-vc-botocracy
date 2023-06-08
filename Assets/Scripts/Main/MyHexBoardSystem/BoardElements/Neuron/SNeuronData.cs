using System;
using ExternBoardSystem.BoardElements;
using Main.Neurons;
using Main.Neurons.UI;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements.Neuron {
    [CreateAssetMenu(menuName = "Neurons/Neuron Data")]
    public class SNeuronData : ScriptableObject, IElementDataProvider<BoardNeuron, MUIBoardNeuron> {
        [SerializeField] private ENeuronType neuronType;

        public ENeuronType Type {
            get => neuronType;
            set => neuronType = value;
        }

        
        [Header("Sprites"), SerializeField] private Sprite boardArtwork;
        [SerializeField] private Sprite UIStackArtwork;
        [SerializeField] private Sprite UIThirdArtwork;
        [SerializeField] private Sprite UISecondArtwork;
        [SerializeField] private Sprite UIFirstArtwork;
        
        [Header("Prefab  Models"), SerializeField] private MUIBoardNeuron boardModel;
        [Header("UI Model"), SerializeField] private MUINeuron UIModel;

        public void SetData(SNeuronData other) {
            Type = other.Type;
            boardArtwork = other.boardArtwork;
            boardModel = other.boardModel;
        }

        public BoardNeuron GetElement() {
            return new BoardNeuron(this);
        }

        public MUIBoardNeuron GetModel() {
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

        #region ActivationFunctions

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

        #endregion

        public override string ToString() {
            return $"{neuronType}";
        }
    }
}