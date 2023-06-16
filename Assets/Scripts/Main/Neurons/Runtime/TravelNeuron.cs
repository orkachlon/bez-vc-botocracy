using System;
using System.Linq;
using System.Threading.Tasks;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;
using Random = UnityEngine.Random;

namespace Main.Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {

        private MUITravelNeuron _uiNeuron;
        private int _turnsToStop;
        private bool _moving;

        public TravelNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling)) {
            _turnsToStop = ((STravelNeuronData) DataProvider).TurnsToStop;
        }

        public override void Activate() {
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Travel);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, StopTravelling);
        }

        public override MUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.Get(DataProvider.GetModel()) as MUITravelNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }

        private async void Travel(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> placementData || placementData.Element.Equals(this)) {
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
                _moving = true;
                await Controller.RemoveNeuron(Position);
                _moving = false;
                Controller.AddElement(NeuronFactory.GetBoardNeuron(ENeuronType.Dummy), Position);
                Controller.AddNeuron(this, randomNeighbor, false);
                Position = randomNeighbor;
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
            if (obj is not BoardElementEventArgs<BoardNeuron> args || args.Element != this || _moving) {
                return;
            }
            UnregisterFromBoard();
        }
    }
}