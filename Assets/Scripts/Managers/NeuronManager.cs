using System;
using System.Collections.Generic;
using System.Linq;
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
            for (var i = 0; i < 10; i++) {
                nextNeurons.Enqueue(Instantiate(GetRandomNeuronPrefab(), nextNeurons.transform.position, Quaternion.identity, nextNeurons.transform));
            }
            _currentNeuron = nextNeurons.Dequeue();
        }

        private void OnDestroy() {
            Tile.OnTileClickedEvent -= PlaceNeuron;
            Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
            Tile.OnTileMouseExitEvent -= HideCurrentNeuron;
            Grid.GridDisabled -= HideCurrentNeuron;
        }

        private void Update() {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                _currentNeuron.Rotate(true);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                _currentNeuron.Rotate(false);
            }
        }

        private void PlaceNeuron(Tile tile) {
            var placingSuccessful = tile.PlaceNeuron(_currentNeuron);
            if (!placingSuccessful) {
                return;
            }

            _currentNeuron.Show();
            _currentNeuron = nextNeurons.Dequeue();
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

        private Neuron GetRandomNeuronPrefab() {
            return _typeToPrefab.Values.ToList()[Random.Range(0, _typeToPrefab.Count)];
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
