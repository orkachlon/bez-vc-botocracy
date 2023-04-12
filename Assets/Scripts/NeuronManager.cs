using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NeuronManager : MonoBehaviour {

    [SerializeField] private Neuron uniNeuronPrefab;
    [SerializeField] private Neuron biNeuronPrefab;
    [SerializeField] private Neuron triNeuronPrefab;

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
        _typeToPrefab = new Dictionary<Neuron.NeuronType, Neuron>() {
            { Neuron.NeuronType.Uni, uniNeuronPrefab },
            { Neuron.NeuronType.Bi, biNeuronPrefab },
            { Neuron.NeuronType.Tri, triNeuronPrefab }
        };
        _currentNeuron = Instantiate(uniNeuronPrefab, Vector3.zero, Quaternion.identity, transform);
        Cursor.visible = false;
        Tile.OnTileClickedEvent += PlaceNeuron;
        Tile.OnTileMouseEnterEvent += SnapNeuronToTile;
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
        _currentNeuron.transform.position = tile.transform.position;
        tile.PlaceNeuron(_currentNeuron);
        _currentNeuron = Instantiate(GetRandomNeuronPrefab(), Utils.GetMousePos(), Quaternion.identity, transform);
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

    public Neuron GetNextNeuron() {
        return uniNeuronPrefab;
    }

    private Neuron GetRandomNeuronPrefab() {
        return _typeToPrefab.Values.ToList()[Random.Range(0, _typeToPrefab.Count)];
    }
}
