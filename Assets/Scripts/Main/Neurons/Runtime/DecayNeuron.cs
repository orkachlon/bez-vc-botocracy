using System;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;
        private MUIDecayNeuron _uiNeuron;

        public DecayNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying)) {
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
        }

        public override void Activate() { }

        public override void BindToBoard(SEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            base.BindToBoard(boardEventManager, controller, position);
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
        }

        public override MUIBoardNeuron Pool() {
            if (_uiNeuron != null) {
                MLogger.LogEditor("Pooled existing neuron!");
            }
            _uiNeuron = MObjectPooler.Instance.Get(DataProvider.GetModel()) as MUIDecayNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }

        private void Decay(EventArgs args) {
            if (args is not BoardElementEventArgs<BoardNeuron> placementArgs || placementArgs.Element == this) {
                return;
            }
            _turnsToDeath--;
            _uiNeuron.PlayTurnAnimation();
            if (_turnsToDeath > 0) {
                return;
            }
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Decay);
            Controller.RemoveNeuron(Position);
        }
    }
}