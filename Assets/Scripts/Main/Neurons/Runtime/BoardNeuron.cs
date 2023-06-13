using System;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public abstract class BoardNeuron : BoardElement {

        protected Hex Position;

        protected SEventManager NeuronEventManager;
        protected SEventManager BoardEventManager;
        protected IBoardNeuronsController Controller;
        
        protected BoardNeuron(SNeuronDataBase dataProvider) : base(dataProvider) { }

        public new SNeuronDataBase DataProvider => base.DataProvider as SNeuronDataBase;

        public abstract void Activate();

        public virtual void BindToNeuronManager(SEventManager neuronEventManager) {
            NeuronEventManager = neuronEventManager;
        }

        public virtual void BindToBoard(SEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            BoardEventManager = boardEventManager;
            BoardEventManager.Register(ExternalBoardEvents.OnAddElement, Connect);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, Disconnect);
            Controller = controller;
            Position = position;
        }

        protected void Connect(BoardNeuron other) {
            var neighbors = Controller.Manipulator.GetNeighbours(Position);
            if (!neighbors.Contains(other.Position)) {
                return;
            }

            if (!IsHoldingConnectionTo(other)) {
                return;
            }
            
            // either our q < other q or (our q == other q && our r < other r)
            NeuronEventManager.Raise(NeuronEvents.OnConnectNeurons, new NeuronConnectionArgs(this, other));
        }

        protected void Disconnect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data);

            foreach (var other in neighbors) {
                if (!IsHoldingConnectionTo(other)) {
                    continue;
                }
                NeuronEventManager.Raise(NeuronEvents.OnDisconnectNeurons, new NeuronConnectionArgs(this, other));
            }
        }

        protected int GetDistanceFromCenter() {
            return new []{
                Mathf.Abs(Position.q), Mathf.Abs(Position.r), Mathf.Abs(Position.s)
            }.Max();
        }

        protected bool IsHoldingConnectionTo(BoardNeuron other) {
            var absQ = Mathf.Abs(Position.q);
            var otherAbsQ = Mathf.Abs(other.Position.q);
            if (absQ != otherAbsQ) {
                return absQ < otherAbsQ;
            }
            
            var absR = Mathf.Abs(Position.r);
            var otherAbsR = Mathf.Abs(other.Position.r);
            return absR < otherAbsR;
        }

        #region EventHandlers

        protected void Connect(EventArgs args) {
            if (args is not BoardElementEventArgs<BoardNeuron> addArgs) {
                return;
            }

            var other = addArgs.Element;
            Connect(other);
        }

        private void Disconnect(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> addArgs || addArgs.Element != this) {
                return;
            }

            Disconnect();

            
        }

        #endregion
    }
}