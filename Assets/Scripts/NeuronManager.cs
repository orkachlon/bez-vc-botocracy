using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuronManager : MonoBehaviour {

    [SerializeField] private Neuron neuronPrefab;

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
        _currentNeuron = Instantiate(neuronPrefab, Vector3.zero, Quaternion.identity, transform);
        Cursor.visible = false;
        Tile.OnTileClickedEvent += SpawnNeuron;
        Tile.OnTileMouseEnterEvent += SnapNeuronToTile;
    }

    private void OnDestroy() {
        Tile.OnTileClickedEvent -= SpawnNeuron;
        Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
    }

    private void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            _currentNeuron.Rotate(true);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            _currentNeuron.Rotate(false);
        }
        // stick neuron to cursor
        // _currentNeuron.transform.position = Utils.GetMousePos();
        // snap position to nearest tile
        // _currentNeuron.transform.position = Grid.Instance.GetNearestTile(Utils.GetMousePos()).transform.position;
    }

    private void SpawnNeuron(Tile tile) {

    }

    private void SnapNeuronToTile(Tile tile) {
        _currentNeuron.transform.position = tile.transform.position;
    }

    public Neuron GetNextNeuron() {
        return neuronPrefab;
    }
}
