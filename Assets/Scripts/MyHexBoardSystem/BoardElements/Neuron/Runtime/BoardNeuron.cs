using System;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Tools.Pooling;
using Core.Utils;
using Events.Board;
using Events.Neuron;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

namespace MyHexBoardSystem.BoardElements.Neuron.Runtime {
    public abstract class BoardNeuron : IBoardNeuron {

        public abstract INeuronDataBase DataProvider { get; }
        IElementDataProvider<IBoardElement, IUIBoardElement> IBoardElement.DataProvider => DataProvider;

        public Hex Position { get; protected set; }
        public bool Connectable { get; protected set; }
        public bool TurnDone { get; protected set; }

        protected SEventManager NeuronEventManager;
        protected SEventManager BoardEventManager;
        protected IBoardNeuronsController Controller;
        protected IUIBoardNeuron UINeuron;
        protected int Holders;
        protected abstract IBoardNeuronConnector Connector { get; set; }


        protected BoardNeuron() {
            Connectable = true;
        }

        public abstract Task Activate();

        public void BindToNeurons(IEventManager neuronEventManager) {
            NeuronEventManager = neuronEventManager as SEventManager;
        }

        public virtual void UnbindFromNeurons() {
            
        }

        public virtual void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            BoardEventManager = boardEventManager as SEventManager;
            if (BoardEventManager == null) {
                MLogger.LogEditor("Couldn't interpret boardEventManager as SEventManager");
                return;
            }
            Controller = controller;
            Position = position;
            RegisterTurnDone();
        }

        public virtual void UnbindFromBoard() {
            UnregisterTurnDone();
        }

        public virtual IUIBoardNeuron Pool() {
            if (Holders == 0) {
                UINeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()); 
            }

            UINeuron.SetRuntimeElementData(this);
            Holders++;
            return UINeuron;
        }

        public virtual void Release() {
            Holders--;
            if (Holders != 0) {
                return;
            }
            MObjectPooler.Instance.Release(UINeuron.GO);
        }

        public virtual async Task AwaitAddition() {
            UINeuron.PlayAddSound();
            await UINeuron.PlayAddAnimation();
            await Connect();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        public virtual async Task AwaitRemoval() {
            UINeuron.PlayRemoveSound();
            await Disconnect();
            await UINeuron.PlayRemoveAnimation();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedRemoveAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        public virtual async Task AwaitMove(Vector3 fromPos, Vector3 toPos) {
            await UINeuron.PlayMoveAnimation(fromPos, toPos);
        }

        public virtual async Task Connect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data)
                .Where(n => n.Connectable);

            var connectionTasks = neighbors.Select((n, i) => Connector.Connect(this, n, i * 50));
            await Task.WhenAll(connectionTasks);
        }

        public virtual async Task Disconnect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data);

            var disconnectionTasks = neighbors
                .Select((other, i) => Connector.Disconnect(this, other, i * 50));

            await Task.WhenAll(disconnectionTasks);
        }

        public virtual Hex[] GetAffectedTiles(Hex hex, INeuronBoardController controller = null) => new Hex[]{};
        public virtual TileBase GetEffectTile() => DataProvider.GetEffectTile();

        protected virtual void RegisterTurnDone() {
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, ReportTurnDone);
            BoardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, ResetTurnIndicator);
        }

        protected virtual void UnregisterTurnDone() {
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, ReportTurnDone);
            BoardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, ResetTurnIndicator);
        }

        protected virtual void ReportTurnDone(EventArgs args = null) {
            if (args is BoardElementEventArgs<IBoardNeuron> neuronArgs && neuronArgs.Element == this || TurnDone) {
                return;
            }
            TurnDone = true;
            BoardEventManager.Raise(ExternalBoardEvents.OnSingleNeuronTurn, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        protected virtual void ResetTurnIndicator(EventArgs obj) {
            TurnDone = false;
        }
    }
}