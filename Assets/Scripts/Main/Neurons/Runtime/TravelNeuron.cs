using System;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {
        
        private int _turnsToStop;
        private SEventManager _boardEventManager;
        private IBoardNeuronsController _controller;
        private Hex _position;
        private bool _moving;

        public TravelNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling)) {
            _turnsToStop = ((STravelNeuronData) DataProvider).TurnsToStop;
        }

        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) {
            _controller = controller;
            _position = BoardManipulationOddR<BoardNeuron>.GetHexCoordinate(cell);
            _boardEventManager = boardEventManager;
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Travel);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveElement, Die);
        }
        
        private void Travel(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> placementData || placementData.Element.Equals(this)) {
                return;
            }

            if (!_controller.Board.HasPosition(_position)) {
                UnregisterFromBoard();
                return;
            }

            // expand to 1 random neighbor
            var neighbours = _controller.Manipulator.GetNeighbours(_position)
                .Where(h => !_controller.Board.GetPosition(h).HasData())
                .ToArray();
            if (neighbours.Length > 0) {
                var randomNeighbor = neighbours[Random.Range(0, neighbours.Length)];
                _moving = true;
                _controller.RemoveElement(_position);
                _moving = false;
                _controller.AddElement(NeuronFactory.GetBoardNeuron(ENeuronType.Dummy), _position);
                _controller.AddNeuron(this, randomNeighbor, false);
                _position = randomNeighbor;
            }
            _turnsToStop--;
            if (_turnsToStop > 0) {
                return;
            }
            UnregisterFromBoard();
        }

        private void UnregisterFromBoard() {
            _boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Travel);
            _boardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, Die);
        }

        private void Die(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> args || args.Element != this || _moving) {
                return;
            }
            UnregisterFromBoard();
        }
    }
}