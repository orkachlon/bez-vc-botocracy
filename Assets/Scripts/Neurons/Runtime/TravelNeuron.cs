using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using MyHexBoardSystem.Events;
using Neurons.Data;
using Neurons.UI;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Random = UnityEngine.Random;

namespace Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {

        private int _turnsToStop;
        private bool _moving;

        public sealed override INeuronDataBase DataProvider { get; }

        public TravelNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling);
            _turnsToStop = ((STravelNeuronData) DataProvider).TurnsToStop;
        }

        public override void Activate() {
            if (BoardEventManager == null) {
                return;
            }
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Travel);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, StopTravelling);
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitRemoval() {
            await UINeuron.PlayRemoveAnimation();
        }

        private async void Travel(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> placementData || placementData.Element.Equals(this)) {
                return;
            }

            if (!Controller.Board.HasPosition(Position)) {
                UnregisterFromBoard();
                return;
            }

            // expand to 1 random neighbor
            var neighbours = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => !Controller.Board.GetPosition(h).HasData())
                .ToArray();
            if (neighbours.Length > 0) {
                var randomNeighbor = neighbours[Random.Range(0, neighbours.Length)];
                Disconnect();
                Controller.MoveNeuron(Position, randomNeighbor);
                Controller.AddElement(NeuronFactory.GetBoardNeuron(ENeuronType.Dummy), Position);
                Position = randomNeighbor;
                Connect();
            }
            else { // stop travelling if you couldn't travel this turn
                _turnsToStop = 0;
            }
            _turnsToStop--;
            if (_turnsToStop > 0) {
                return;
            }
            UnregisterFromBoard();
        }

        private void UnregisterFromBoard() {
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Travel);
            BoardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, StopTravelling);
        }

        private void StopTravelling(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> args || args.Element != this || _moving) {
                return;
            }
            UnregisterFromBoard();
        }
    }
}