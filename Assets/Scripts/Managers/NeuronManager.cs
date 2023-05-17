using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Neuron;
using Neurons;
using Tiles;
using UnityEngine;
using UnityEngine.Assertions;
using Grid = Grids.Grid;
using Random = UnityEngine.Random;

namespace Managers {
    public class NeuronManager : MonoBehaviour {

        [SerializeField] private List<Neuron> neurons;
        [SerializeField] private NeuronQueue nextNeurons;
        
        [SerializeField] private BoardElementsController elementsController;
        [SerializeField] private NeuronData placeNeuronData;
        public static NeuronManager Instance { get; private set; }
        public static event Action OnNeuronPlaced;

        private Dictionary<Neuron.NeuronType, Neuron> _typeToPrefab;
        private Neuron _currentNeuron;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            
            _typeToPrefab = neurons.ToDictionary(n => n.Type);

            Tile.OnTileClickedEvent += PlaceNeuron;
            Tile.OnTileMouseOverEvent += SnapNeuronToTile;
            Tile.OnTileMouseExitEvent += HideCurrentNeuron;
            Grid.GridDisabled += HideCurrentNeuron;
        }

        private void Start() {
            Assert.IsNotNull(neurons);
            RewardNeurons(10);
            _currentNeuron = nextNeurons.Dequeue();
            
            elementsController.SetElementProvider(placeNeuronData);
        }

        private void OnDestroy() {
            Tile.OnTileClickedEvent -= PlaceNeuron;
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

        private void PlaceNeuron(Tile tile) {
            var placingSuccessful = tile.PlaceNeuron(_currentNeuron);
            if (!placingSuccessful) {
                return;
            }

            _currentNeuron.Show();
            _currentNeuron = nextNeurons.Dequeue();
            // elementsController.SetElementProvider();
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
            tile.PlaceNeuron(Instantiate(GetRandomNeuronPrefab(), tile.transform.position, Quaternion.identity,
                tile.transform));
        }

        public Neuron GetRandomNeuronPrefab() {
            return _typeToPrefab.Values.ToList()[Random.Range(0, _typeToPrefab.Count)];
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
