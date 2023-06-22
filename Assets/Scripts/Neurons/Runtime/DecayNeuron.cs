using System;
using System.Threading.Tasks;
using Animation;
using Events.Board;
using Events.Neuron;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;

        public sealed override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }

        public DecayNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying);
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
            Connector = NeuronFactory.GetConnector();
        }

        public override async Task Activate() {
            await AnimationManager.WaitForElement(this);
            ReportTurnDone();
        }

        public override void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            base.BindToBoard(boardEventManager, controller, position);
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
            BoardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, ResetTurnIndicator);
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitAddition() {
            UINeuron.PlayAddSound();
            await Task.WhenAll(UINeuron.PlayAddAnimation(), Connect());
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        private void Decay(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> placementArgs) {
                return;
            }

            if (placementArgs.Element == this) {
                ReportTurnDone();
                return;
            }
            
            _turnsToDeath--;
            UINeuron.PlayTurnAnimation();
            if (_turnsToDeath <= 0) {
                BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Decay);
                Controller.RemoveNeuron(Position);
            }
            ReportTurnDone();
        }

        protected override void RegisterTurnDone() { }
    }
}