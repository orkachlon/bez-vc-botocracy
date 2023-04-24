using System.Collections.Generic;
using System.Linq;
using Neurons;
using Tiles;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Managers {
    public class NeuronManager : MonoBehaviour {

        [SerializeField] private List<Neuron> neurons;

        private Dictionary<Neuron.NeuronType, Neuron> _typeToPrefab;

        public static NeuronManager Instance { get; private set; }

        private Queue<Neuron> _nextNeurons;

        private Neuron _currentNeuron;
    
        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
        }

        private void Start() {
            Assert.IsNotNull(neurons);
            _typeToPrefab = neurons.ToDictionary(n => n.Type);
            _currentNeuron = Instantiate(GetRandomNeuronPrefab(), Vector3.zero, Quaternion.identity, transform);
            Cursor.visible = false;
            Tile.OnTileClickedEvent += PlaceNeuron;
            Tile.OnTileMouseOverEvent += SnapNeuronToTile;
        }

        private void OnDestroy() {
            Tile.OnTileClickedEvent -= PlaceNeuron;
            Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
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
            if (placingSuccessful) {
                // hold next neuron
                _currentNeuron = Instantiate(GetRandomNeuronPrefab(), Utils.Utils.GetMousePos(), Quaternion.identity, transform);
            }
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

        private Neuron GetRandomNeuronPrefab() {
            return _typeToPrefab.Values.ToList()[Random.Range(0, _typeToPrefab.Count)];
        }
    }
}
