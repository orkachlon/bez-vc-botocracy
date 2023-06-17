using System;
using System.Threading.Tasks;
using Core.Tools.Pooling;
using Core.Utils;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.Events;
using Neurons.Data;
using Neurons.UI;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;
        private MUIDecayNeuron _uiNeuron;

        public DecayNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying)) {
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
        }

        public override void Activate() { }

        public override void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            base.BindToBoard(boardEventManager, controller, position);
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
        }

        public override IUIBoardNeuron Pool() {
            _uiNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetModel()) as MUIDecayNeuron;
            return _uiNeuron;
        }

        public override async Task AwaitNeuronRemoval() {
            await _uiNeuron.PlayRemoveAnimation();
        }

        private void Decay(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> placementArgs || placementArgs.Element == this) {
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