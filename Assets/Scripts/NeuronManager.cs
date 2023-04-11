using System;
using UnityEngine;

public class NeuronManager : MonoBehaviour {

    [SerializeField] private Neuron neuronPrefab;

    public static NeuronManager Instance { get; private set; }

    private Neuron _nextNeuron;
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }

    private void Start() {
        Tile.OnTileClickedEvent += SpawnNeuron;
    }

    private void OnDestroy() {
        Tile.OnTileClickedEvent -= SpawnNeuron;
    }

    private void Update() {
        // if (Input.GetAxis("Mouse ScrollWheel") > 0) {
        //     _nextNeuron.Rotate(true);
        // }
        // if (Input.GetAxis("Mouse ScrollWheel") < 0) {
        //     _nextNeuron.Rotate(false);
        // }
    }

    private void SpawnNeuron(Tile tile) {

    }

    public Neuron GetNextNeuron() {
        return neuronPrefab;
    }
}
