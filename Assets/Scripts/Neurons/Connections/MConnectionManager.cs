using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using Core.Tools.Pooling;
using Core.Utils.Singleton;
using Events.Neuron;
using MyHexBoardSystem.BoardSystem;
using Types.Neuron.Connections;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Connections {
    public class MConnectionManager : MSingleton<MConnectionManager>, IConnectionManager {

        [SerializeField] private MNeuronConnection connectionPrefab;
        [SerializeField] private MNeuronBoardController controller;
        
        [Header("Event Managers"), SerializeField]
        private SEventManager neuronEventManager;

        private readonly ConcurrentDictionary<string, MNeuronConnection> _connections = new();
        private static readonly SemaphoreSlim ConnectionLockInner = new(1, 1);

        public SemaphoreSlim ConnectionLock => ConnectionLockInner;

        
        #region UnityMethods

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnConnectNeurons, AddConnection);
            neuronEventManager.Register(NeuronEvents.OnDisconnectNeurons, RemoveConnection);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnConnectNeurons, AddConnection);
            neuronEventManager.Unregister(NeuronEvents.OnDisconnectNeurons, RemoveConnection);
        }

        #endregion

        #region EventHandlers

        private async void AddConnection(EventArgs obj) {
            if (obj is not NeuronConnectionArgs connectionArgs) {
                return;
            }
            var n1 = connectionArgs.Neuron1;
            var n2 = connectionArgs.Neuron2;

            await Connect(n1, n2);
        }

        private async void RemoveConnection(EventArgs args) {
            if (args is not NeuronConnectionArgs connectionArgs) {
                return;
            }

            var n1 = connectionArgs.Neuron1;
            var n2 = connectionArgs.Neuron2;

            await Disconnect(n1, n2);
        }

        #endregion

        public async Task Connect(IBoardNeuron n1, IBoardNeuron n2) {
            var key = GetConnectionKey(n1, n2);
            if (DoesConnectionExist(n1, n2)) {
                return;
            }

            var newConnection = MObjectPooler.Instance.GetPoolable(connectionPrefab);
            newConnection.Default();
            _connections[key] = newConnection;
            await newConnection.Connect(controller, n1, n2);
            await AnimationManager.WaitForElement(newConnection);
        }

        public async Task Disconnect(IBoardNeuron n1, IBoardNeuron n2) {
            if (!DoesConnectionExist(n1, n2)) {
                return;
            }

            var connection = GetConnection(n1, n2);
            await connection.Disconnect();
            await AnimationManager.WaitForElement(connection);
            MObjectPooler.Instance.ReleasePoolable(connection);
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