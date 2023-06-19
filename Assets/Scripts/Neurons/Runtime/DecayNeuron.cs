using System;
using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.Events;
using Neurons.Data;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;

        public sealed override INeuronDataBase DataProvider { get; }

        public DecayNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying);
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
        }

        public override void Activate() { }


        public override void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            base.BindToBoard(boardEventManager, controller, position);
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitRemoval() {
            await UINeuron.PlayRemoveAnimation();
        }

        private void Decay(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> placementArgs || placementArgs.Element == this) {
                return;
            }
            _turnsToDeath--;
            UINeuron.PlayTurnAnimation();
            if (_turnsToDeath > 0) {
                return;
            }
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Decay);
            Controller.RemoveNeuron(Position);
        }
    }
}