using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Events.Board;
using Events.Neuron;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neurons.Runtime {
    public class TravelNeuron : BoardNeuron {

        public int TurnsToStop { get; private set; }
        private Hex _prevPos;

        public sealed override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }

        protected static readonly ConcurrentDictionary<Hex, TravelNeuron> PickedPositions = new();
        
        protected MUITravelNeuron UITravelNeuron => UINeuron as MUITravelNeuron;


        public TravelNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Travelling);
            TurnsToStop = ((STravelNeuronData) DataProvider).TurnsToStop;
            Connector = NeuronFactory.GetConnector();
        }

        public override Task Activate() {
            if (BoardEventManager == null) {
                return Task.CompletedTask;
            }
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElementTurnDone, Travel);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, OnRemoved);
            BoardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, ResetTurnIndicator);
            NeuronEventManager.Register(NeuronEvents.OnTravellersReady, BeginTravel);
            NeuronEventManager.Raise(NeuronEvents.OnTravelNeuronReady, new BoardElementEventArgs<IBoardNeuron>(this, Position));
            ReportTurnDone();
            return Task.CompletedTask;
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UITravelNeuron.SetRuntimeElementData(this);
            return UITravelNeuron;
        }

        public override Task AwaitMove(Vector3 fromPos, Vector3 toPos) {
            UITravelNeuron.PlayMoveSound();
            return base.AwaitMove(fromPos, toPos);
        }

        #region EventHandlers

        private async void Travel(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> placementData) {
                return;
            }

            if (placementData.Element.Equals(this)) {
                if (!CanTravel()) {
                    TurnsToStop = 0;
                    NeuronEventManager.Raise(NeuronEvents.OnTravelNeuronStopped, new BoardElementEventArgs<IBoardNeuron>(this, Position));
                    UITravelNeuron.DepleteTurns();
                    UnregisterFromBoard();
                    base.RegisterTurnDone();
                }
                return;
            }

            if (!Controller.Board.HasPosition(Position)) {
                UnregisterFromBoard();
                ReportTurnDone();
                return;
            }

            // expand to 1 random neighbor
            var neighbours = GetEmptyNeighbors();
            if (neighbours.Length > 0) {
                var randomNeighbor = neighbours[Random.Range(0, neighbours.Length)];
                PickedPositions[randomNeighbor] = this;
                _prevPos = Position;
                Connectable = false;
                await Disconnect();
                Position = randomNeighbor;
                NeuronEventManager.Raise(NeuronEvents.OnTravelNeuronReady, new BoardElementEventArgs<IBoardNeuron>(this, Position));
                return;
            }
            // stop travelling if you couldn't travel this turn
            StopTravelling();
        }

        private async void BeginTravel(EventArgs obj) {
            if (_prevPos == Hex.zero) {
                return;
            }

            UITravelNeuron.PlayTurnAnimation();
            await TravelTo(_prevPos, Position);
            if (!CanTravel()) {
                StopTravelling();
                return;
            }
            ReportTurnDone();
        }

        private void OnRemoved(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> args || args.Element != this) {
                return;
            }
            UnregisterFromBoard();
        }

        #endregion

        private async Task TravelTo(Hex from, Hex to) {
            await Controller.MoveNeuron(from, to);
            Connectable = true;
            await Controller.AddElement(NeuronFactory.GetBoardNeuron(ENeuronType.Dummy), from);
            PickedPositions.TryRemove(to, out _);
            await Connect();
            TurnsToStop--;
        }

        protected virtual Hex[] GetEmptyNeighbors() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => !Controller.Board.GetPosition(h).HasData() && !PickedPositions.ContainsKey(h))
                .ToArray();
            return neighbours;
        }

        private void UnregisterFromBoard() {
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementTurnDone, Travel);
            BoardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, OnRemoved);
            NeuronEventManager.Unregister(NeuronEvents.OnTravellersReady, BeginTravel);
        }

        private bool CanTravel() {
            return TurnsToStop > 0 && GetEmptyNeighbors().Length > 0;
        }

        private void StopTravelling() {
            UnregisterFromBoard();
            TurnsToStop = 0;
            UITravelNeuron.DepleteTurns();
            NeuronEventManager.Raise(NeuronEvents.OnTravelNeuronStopped, new BoardElementEventArgs<IBoardNeuron>(this, Position));
            ReportTurnDone();
            base.RegisterTurnDone();
        }

        protected override void RegisterTurnDone() { }
    }
}