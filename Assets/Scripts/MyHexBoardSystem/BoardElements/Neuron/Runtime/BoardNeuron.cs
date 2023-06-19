using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Tools.Pooling;
using Core.Utils;
using MyHexBoardSystem.Events;
using Neurons;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

namespace MyHexBoardSystem.BoardElements.Neuron.Runtime {
    public abstract class BoardNeuron : IBoardNeuron {

        public abstract INeuronDataBase DataProvider { get; }
        IElementDataProvider<IBoardElement, IUIBoardElement> IBoardElement.DataProvider => DataProvider;

        public Types.Hex.Coordinates.Hex Position { get; protected set; }
        public bool Connectable { get; protected set; }

        protected SEventManager NeuronEventManager;
        protected SEventManager BoardEventManager;
        protected IBoardNeuronsController Controller;
        protected IUIBoardNeuron UINeuron;


        protected BoardNeuron() {
            Connectable = true;
        }

        public abstract Task Activate();

        public virtual void BindToNeuronManager(IEventManager neuronEventManager) {
            NeuronEventManager = neuronEventManager as SEventManager;
        }

        public virtual void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller, Types.Hex.Coordinates.Hex position) {
            BoardEventManager = boardEventManager as SEventManager;
            if (BoardEventManager == null) {
                MLogger.LogEditor("Couldn't interpret boardEventManager as SEventManager");
                return;
            }
            BoardEventManager.Register(ExternalBoardEvents.OnAddElement, Connect);
            BoardEventManager.Register(ExternalBoardEvents.OnRemoveElement, Disconnect);
            Controller = controller;
            Position = position;
        }

        public virtual IUIBoardNeuron Pool() {
            UINeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel());
            return UINeuron;
        }

        public virtual void Release() {
            if (UINeuron == null) {
                return;
            }
            MObjectPooler.Instance.Release(UINeuron.GO);
        }

        public abstract Task AwaitRemoval();

        // {
        //     if (UINeuron == null) {
        //         MLogger.LogEditor("Tried to await remove animation of null ui neuron");
        //         return;
        //     }
        //     await UINeuron.PlayRemoveAnimation();
        // }

        public virtual void Connect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data);

            foreach (var other in neighbors) {
                NeuronEventManager.Raise(NeuronEvents.OnConnectNeurons, new NeuronConnectionArgs(this, other));
            }
        }

        public virtual void Disconnect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data);

            foreach (var other in neighbors) {
                NeuronEventManager.Raise(NeuronEvents.OnDisconnectNeurons, new NeuronConnectionArgs(this, other));
            }
        }

        #region EventHandlers

        protected void Connect(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> addArgs || addArgs.Element != this) {
                return;
            }

            Connect();
        }

        private void Disconnect(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> addArgs || addArgs.Element != this) {
                return;
            }
            BoardEventManager.Unregister(ExternalBoardEvents.OnAddElement, Connect);
            BoardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, Disconnect);

            Disconnect();
        }

        private void Reconnect(EventArgs obj) {
            if (obj is not BoardElementMovedEventArgs<IBoardNeuron> moveArgs || moveArgs.Element != this) {
                return;
            }
            Connect();
        }

        #endregion
    }
}