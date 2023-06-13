using System;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.Neurons.Connections {
    public class MConnectionManager : MonoBehaviour {

        [SerializeField] private MNeuronConnection connectionPrefab;
        [SerializeField] private MNeuronBoardController controller;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        private readonly Dictionary<BoardNeuron, MNeuronConnection> _connections = new Dictionary<BoardNeuron, MNeuronConnection>();

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnConnectNeurons, AddConnection);
            neuronEventManager.Register(NeuronEvents.OnDisconnectNeurons, RemoveConnection);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnConnectNeurons, AddConnection);
            neuronEventManager.Unregister(NeuronEvents.OnDisconnectNeurons, RemoveConnection);
        }

        private void AddConnection(EventArgs obj) {
            if (obj is not NeuronConnectionArgs connectionArgs) {
                return;
            }
            
            var newConnection = MObjectPooler.Instance.Get(connectionPrefab.gameObject).GetComponent<MNeuronConnection>();
            _connections[connectionArgs.Neuron1] = newConnection;
            newConnection.Connect(controller, connectionArgs.Neuron1, connectionArgs.Neuron2);
        }
        
        private void RemoveConnection(EventArgs args) {
            if (args is not NeuronConnectionArgs connectionArgs) {
                return;
            }

            var connection = _connections[connectionArgs.Neuron1];
            MObjectPooler.Instance.Release(connection.gameObject);
            _connections.Remove(connectionArgs.Neuron1);
        }
    }
}