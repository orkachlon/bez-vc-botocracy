using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Ui.Util;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using Tiles;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;
using Grid = Grids.Grid;
using Random = UnityEngine.Random;

namespace Managers {
    public class NeuronManager : MonoBehaviour {

        [SerializeField] private NeuronQueue nextNeurons;

        [SerializeField] private MUITileMapInputHandler uiTileMapInputHandler;
        [SerializeField] private MBoardElementsController<BoardNeuron> elementsController;
        public static NeuronManager Instance { get; private set; }
        public static event Action OnNeuronPlaced;

        private Dictionary<Neuron.ENeuronType, Neuron> _typeToPrefab;
        private Neuron _currentNeuron;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            
            // Tile.OnTileClickedEvent += PlaceNeuron;
            Tile.OnTileMouseOverEvent += SnapNeuronToTile;
            Tile.OnTileMouseExitEvent += HideCurrentNeuron;
            Grid.GridDisabled += HideCurrentNeuron;
            
            // new events
            uiTileMapInputHandler.OnClickTile += PlaceNeuron;
        }

        private void Start() {
            RewardNeurons(10);
            _currentNeuron = nextNeurons.Dequeue();
            // add the initial neuron
            elementsController.SetElementProvider(MNeuronBindings.DataFromType(Neuron.ENeuronType.Invulnerable));
            elementsController.AddStartingElement(new BoardNeuron(MNeuronBindings.DataFromType(Neuron.ENeuronType.Invulnerable)), new Hex(0, 0));
            
            elementsController.SetElementProvider(MNeuronBindings.DataFromType(EnumUtil.GetRandom<Neuron.ENeuronType>()));
        }

        private void OnDestroy() {
            // Tile.OnTileClickedEvent -= PlaceNeuron;
            Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
            Tile.OnTileMouseExitEvent -= HideCurrentNeuron;
            Grid.GridDisabled -= HideCurrentNeuron;
        }

        private void Update() {
            // if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            //     _currentNeuron.Rotate(true);
            // }
            // if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            //     _currentNeuron.Rotate(false);
            // }
        }

        private void PlaceNeuron(Vector3Int cell) {
            // var placingSuccessful = tile.PlaceNeuron(_currentNeuron);
            // if (!placingSuccessful) {
                // return;
            // }

            // _currentNeuron.Show();
            // _currentNeuron = nextNeurons.Dequeue();
            // make this actually update the data
            elementsController.SetElementProvider(MNeuronBindings.DataFromType(EnumUtil.GetRandom<Neuron.ENeuronType>()));
            OnNeuronPlaced?.Invoke();
        }

        private void HideCurrentNeuron(Tile tile) {
            if (_currentNeuron == null) {
                return;
            }
            _currentNeuron.Hide();
        }
        
        private void HideCurrentNeuron() {
            if (_currentNeuron == null) {
                return;
            }
            _currentNeuron.Hide();
        }

        private void SnapNeuronToTile(Tile tile) {
            _currentNeuron.transform.position = tile.transform.position;
            if (tile.IsEmpty()) {
                _currentNeuron.Show();
            }
            else {
                _currentNeuron.Hide();
            }
        }

        public void PlaceFirstNeuron(Tile tile) {
            throw new NotImplementedException("NeuronManager::PlaceFirstNeuron isn't implemented!");
        }
        
        public void RewardNeurons(int numOfNeurons) {
            nextNeurons.Enqueue(numOfNeurons);
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
