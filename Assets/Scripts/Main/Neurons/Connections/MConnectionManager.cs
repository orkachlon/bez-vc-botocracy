using System;
using System.Collections.Generic;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.Neurons.Connections {
    public class MConnectionManager : MonoBehaviour {
        
        
        
        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;

        private HashSet<Connection> _connections = new HashSet<Connection>();

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateConnections);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateConnections);
        }

        private void UpdateConnections(EventArgs obj) {
            if (obj is BoardElementEventArgs<BoardNeuron> addArgs) {
                
            }
            
        }
    }

    internal struct Connection {
        public Hex Pos1;
        public Hex Pos2;
        public BoardNeuron Neuron1;
        public BoardNeuron Neuron2;

        public Connection(Hex pos1, Hex pos2, BoardNeuron neuron1, BoardNeuron neuron2) {
            Pos1 = pos1;
            Pos2 = pos2;
            Neuron1 = neuron1;
            Neuron2 = neuron2;
        }
    }
}