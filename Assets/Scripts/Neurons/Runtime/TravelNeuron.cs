using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using MyHexBoardSystem.Events;
using Neurons.Data;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;
using Random = UnityEngine.Random;

namespace Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {

        private MUITravelNeuron _uiNeuron;
        private int _turnsToStop;
        private bool _moving;

        public TravelNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling)) {
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
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUITravelNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
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