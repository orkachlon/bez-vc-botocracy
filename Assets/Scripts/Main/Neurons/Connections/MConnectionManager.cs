using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils;
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

        private readonly Dictionary<int, MNeuronConnection> _connections = new();

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

            var key = GetConnectionKey(connectionArgs.Neuron1, connectionArgs.Neuron2);
            if (_connections.ContainsKey(key)) {
                return;
            }
            var newConnection = MObjectPooler.Instance.Get(connectionPrefab.gameObject).GetComponent<MNeuronConnection>();
            _connections[key] = newConnection;
            newConnection.Connect(controller, connectionArgs.Neuron1, connectionArgs.Neuron2);
        }
        
        private void RemoveConnection(EventArgs args) {
            if (args is not NeuronConnectionArgs connectionArgs) {
                return;
            }

            var key = GetConnectionKey(connectionArgs.Neuron1, connectionArgs.Neuron2);
            if (!_connections.ContainsKey(key)) {
                MLogger.LogEditor("Tried to remove connection that doesn't exist");
                return;
            }
            var connection = _connections[key];
            MObjectPooler.Instance.Release(connection.gameObject);
            _connections.Remove(key);
        }

        private int GetConnectionKey(BoardNeuron n1, BoardNeuron n2) {
            return n1.Position.GetHashCode() + n2.Position.GetHashCode();
        }
    }
}