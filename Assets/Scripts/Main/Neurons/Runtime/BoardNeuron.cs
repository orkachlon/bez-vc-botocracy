using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public abstract class BoardNeuron : BoardElement {

        public Hex Position { get; protected set; }
        // public MUIBoardNeuron UINeuron { get; protected set; }

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

        public abstract MUIBoardNeuron Pool();
        // {
        //     UINeuron = MObjectPooler.Instance.Get(DataProvider.GetModel());
        //     return UINeuron;
        // }

        public abstract Task AwaitNeuronRemoval();
        // {
        //     if (UINeuron == null) {
        //         MLogger.LogEditor("Tried to await remove animation of null ui neuron");
        //         return;
        //     }
        //     await UINeuron.PlayRemoveAnimation();
        // }

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