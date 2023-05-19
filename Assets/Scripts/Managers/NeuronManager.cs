using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Ui.Particles;
using ExternBoardSystem.Ui.Util;
using MyHexBoardSystem.BoardElements.Neuron;
using MyHexBoardSystem.UI;
using Neurons;
using Tiles;
using UnityEngine;
using Utils;
using Grid = Grids.Grid;

namespace Managers {
    public class NeuronManager : MonoBehaviour {

        [Header("Neuron Queue")]
        [SerializeField] private NeuronQueue.NeuronQueueController nextNeurons;
        
        [Header("Board Input")]
        [SerializeField] private MUITileMapInputHandler uiTileMapInputHandler;
        [SerializeField] private MUITileMapHoverHandler uiTileMapHoverHandler;
        
        [Header("Board Element Controller")]
        [SerializeField] private MBoardElementsController<BoardNeuron, MUIBoardNeuron> elementsController;
        
        public BoardNeuron CurrentNeuron { get; private set; }
        public static NeuronManager Instance { get; private set; }
        public static event Action OnNeuronPlaced;
        public event Action OnNoMoreNeurons;

        private MUINeuron CurrentUINeuron { get; set; }
        private Dictionary<Neuron.ENeuronType, Neuron> _typeToPrefab;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            
            // Grid.GridDisabled += HideCurrentNeuron;
            
            // new events
            // uiTileMapInputHandler.OnClickTile += PlaceNeuron;
            elementsController.OnPlaceElement += NextNeuron;
            // hover smoothly with mouse, but mark the tile below
            uiTileMapHoverHandler.OnHoverTile += i => {};
        }

        private void Start() {
            // add some neurons to the queue
            RewardNeurons(10);
            // place the initial neuron
            var invulnerableBoardNeuron = GetNeuron(Neuron.ENeuronType.Invulnerable);
            elementsController.SetElementProvider(invulnerableBoardNeuron.ElementData);
            elementsController.AddStartingElement(invulnerableBoardNeuron, new Hex(0, 0));
            NextNeuron(null, Vector3Int.zero);
            
            elementsController.SetElementProvider(CurrentNeuron.ElementData);
        }

        private void OnDestroy() {
            // Tile.OnTileClickedEvent -= PlaceNeuron;
            // Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
            // Tile.OnTileMouseExitEvent -= HideCurrentNeuron;
            // Grid.GridDisabled -= HideCurrentNeuron;
            uiTileMapInputHandler.OnClickTile -= PlaceNeuron;
            elementsController.OnAddElement -= NextNeuron;
        }
        
        private void PlaceNeuron(Vector3Int cell) {
            // var placingSuccessful = tile.PlaceNeuron(CurrentNeuron);
            // if (!placingSuccessful) {
                // return;
            // }

            // CurrentNeuron.Show();
            // CurrentNeuron = nextNeurons.Dequeue();
            // make this actually update the data
            elementsController.SetElementProvider(MNeuronTypeToBoardData.GetNeuronData(EnumUtil.GetRandom<Neuron.ENeuronType>()));
            OnNeuronPlaced?.Invoke();
        }

        private void NextNeuron(BoardNeuron boardNeuron, Vector3Int cell) {
            // todo handle neurons being placed by other neurons correctly
            CurrentNeuron = nextNeurons.Dequeue();
            if (CurrentNeuron == null) {
                elementsController.SetElementProvider(null);
                return;
            }
            
            if (nextNeurons.Count == 0) {
                OnNoMoreNeurons?.Invoke();
            }
            elementsController.SetElementProvider(CurrentNeuron.ElementData);
        }

        private void HideCurrentNeuron(Tile tile) {
            if (CurrentNeuron == null) {
                return;
            }
            CurrentUINeuron.Hide();
        }
        
        private void HideCurrentNeuron() {
            if (CurrentNeuron == null) {
                return;
            }
            CurrentUINeuron.Hide();
        }

        private void SnapNeuronToTile(Tile tile) {
            CurrentUINeuron.transform.position = tile.transform.position;
            if (tile.IsEmpty()) {
                CurrentUINeuron.Show();
            }
            else {
                CurrentUINeuron.Hide();
            }
        }

        public void PlaceFirstNeuron(Tile tile) {
            throw new NotImplementedException("NeuronManager::PlaceFirstNeuron isn't implemented!");
        }
        
        public void RewardNeurons(int numOfNeurons) {
            nextNeurons.Enqueue(numOfNeurons);
        }

        public BoardNeuron GetNeuron(Neuron.ENeuronType neuronType) {
            var data = MNeuronTypeToBoardData.GetNeuronData(neuronType);
            return new BoardNeuron(data);
        }

        public BoardNeuron GetRandomNeuron() {
            var rndType = EnumUtil.GetRandom<Neuron.ENeuronType>();
            var data = MNeuronTypeToBoardData.GetNeuronData(rndType);
            return new BoardNeuron(data);
        }

        public void HandleGameStateChanged(GameManager.GameState state) {
            switch (state) {
                case GameManager.GameState.InitGrid:
                case GameManager.GameState.EventTurn:
                    break;
                case GameManager.GameState.PlayerTurn:
                    break;
                case GameManager.GameState.EventEvaluation:
                    break;
                case GameManager.GameState.StatTurn:
                    break;
                case GameManager.GameState.Win:
                    break;
                case GameManager.GameState.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
