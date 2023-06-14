using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public abstract class BoardNeuron : BoardElement {

        public Hex Position { get; protected set; }

        protected bool Connectable;

        protected SEventManager NeuronEventManager;
        protected SEventManager BoardEventManager;
        protected IBoardNeuronsController Controller;

        protected BoardNeuron(SNeuronDataBase dataProvider) : base(dataProvider) {
            Connectable = true;
        }

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

        protected virtual void Connect(BoardNeuron other) {
            if (!Controller.Board.HasPosition(Position)) {
                MLogger.LogEditor("Tried to connect from position that doesn't exist");
                return;
            }
            var neighbors = Controller.Manipulator.GetNeighbours(Position);
            if (!neighbors.Contains(other.Position) || !other.Connectable) {
                return;
            }

            NeuronEventManager.Raise(NeuronEvents.OnConnectNeurons, new NeuronConnectionArgs(this, other));
        }

        protected virtual void Disconnect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data);

            foreach (var other in neighbors) {
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

            Connect(addArgs.Element);
        }

        private void Disconnect(EventArgs obj) {
            if (obj is not BoardElementEventArgs<BoardNeuron> addArgs || addArgs.Element != this) {
                return;
            }
            BoardEventManager.Unregister(ExternalBoardEvents.OnAddElement, Connect);
            BoardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, Disconnect);

            Disconnect();
        }

        #endregion
    }
}