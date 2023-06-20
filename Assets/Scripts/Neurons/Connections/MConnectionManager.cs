using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Tools.Pooling;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Connections {
    public class MConnectionManager : MonoBehaviour {

        [SerializeField] private MNeuronConnection connectionPrefab;
        [SerializeField] private MNeuronBoardController controller;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        private readonly ConcurrentDictionary<string, MNeuronConnection> _connections = new();

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
            var n1 = connectionArgs.Neuron1;
            var n2 = connectionArgs.Neuron2;

            var key = GetConnectionKey(n1, n2);
            if (DoesConnectionExist(n1, n2)) {
                return;
            }
            var newConnection = MObjectPooler.Instance.Get(connectionPrefab.gameObject).GetComponent<MNeuronConnection>();
            _connections[key] = newConnection;
            newConnection.Connect(controller, n1, n2);
        }
        
        private void RemoveConnection(EventArgs args) {
            if (args is not NeuronConnectionArgs connectionArgs) {
                return;
            }

            var n1 = connectionArgs.Neuron1;
            var n2 = connectionArgs.Neuron2;

            if (!DoesConnectionExist(n1, n2)) {
                return;
            }
            var connection = GetConnection(n1, n2);
            MObjectPooler.Instance.Release(connection.gameObject);
            RemoveConnection(n1, n2);
        }

        private string GetConnectionKey(IBoardNeuron n1, IBoardNeuron n2) {
            return $"{n1.Position} - {n2.Position}";
        }

        private bool DoesConnectionExist(IBoardNeuron n1, IBoardNeuron n2) {
            return _connections.ContainsKey(GetConnectionKey(n1, n2)) || _connections.ContainsKey(GetConnectionKey(n2, n1));
        }

        private MNeuronConnection GetConnection(IBoardNeuron n1, IBoardNeuron n2) {
            if (!DoesConnectionExist(n1, n2)) {
                return null;
            }
        
            var key = GetConnectionKey(n1, n2);
            return _connections.ContainsKey(key) ? _connections[key] : _connections[GetConnectionKey(n2, n1)];
        }

        private void RemoveConnection(IBoardNeuron n1, IBoardNeuron n2) {
            if (!DoesConnectionExist(n1, n2)) {
                return;
            }
            var key = GetConnectionKey(n1, n2);
            if (_connections.ContainsKey(key)) {
                _connections.Remove(key, out _);
                return;
            }

            _connections.Remove(GetConnectionKey(n2, n1), out _);
        }
    }
}