using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Animation;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.Events;
using Neurons.Data;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Random = UnityEngine.Random;

namespace Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {

        private int _turnsToStop;
        private bool _moving;

        public sealed override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }

        protected static readonly ConcurrentDictionary<Hex, TravelNeuron> PickedPositions = new();


        public TravelNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling);
            _turnsToStop = ((STravelNeuronData) DataProvider).TurnsToStop;
            Connector = NeuronFactory.GetConnector();
        }

        public override Task Activate() {
            if (BoardEventManager == null) {
                return Task.CompletedTask;;
            }
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Travel);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, StopTravelling);
            return Task.CompletedTask;
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
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
            var neighbours = GetEmptyNeighbors();
            if (neighbours.Length > 0) {
                var randomNeighbor = neighbours[Random.Range(0, neighbours.Length)];
                PickedPositions[randomNeighbor] = this;
                var prevPos = Position;
                await Disconnect();
                Position = randomNeighbor;
                await AnimationManager.WaitForAll(PickedPositions.Values);
                await TravelTo(prevPos, randomNeighbor);
            } else { // stop travelling if you couldn't travel this turn
                _turnsToStop = 0;
                AnimationManager.Register(this, Connect());
            }

            
            if (_turnsToStop > 0) {
                return;
            }
            UnregisterFromBoard();
        }

        private Hex[] GetEmptyNeighbors() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => !Controller.Board.GetPosition(h).HasData() && !PickedPositions.ContainsKey(h))
                .ToArray();
            return neighbours;
        }

        private async Task TravelTo(Hex from, Hex to) {
            await Controller.MoveNeuron(from, to);
            await AnimationManager.WaitForAll();
            var dummy = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
            AnimationManager.Register(this, Controller.AddElement(dummy, from));
            await AnimationManager.WaitForAll(PickedPositions.Values);
            PickedPositions.TryRemove(to, out _);
            await Connect();
            _turnsToStop--;
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