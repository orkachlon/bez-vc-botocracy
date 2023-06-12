using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;
        private SEventManager _boardEventManager;
        private IBoardNeuronsController _controller;
        private Hex _position;

        public DecayNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying)) {
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
        }

        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) {
            _controller = controller;
            _position = BoardManipulationOddR<BoardNeuron>.GetHexCoordinate(cell);
            _boardEventManager = boardEventManager;
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
        }

        private void Decay(EventArgs args) {
            _turnsToDeath--;
            if (_turnsToDeath > 0) {
                return;
            }
            _boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Decay);
            _controller.RemoveElement(_position);
        }
    }
}