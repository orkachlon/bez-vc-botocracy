using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.Events;
using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class DecayNeuron : BoardNeuron {

        private int _turnsToDeath;

        public DecayNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Decaying)) {
            _turnsToDeath = ((SDecayingNeuronData) DataProvider).TurnsToDeath;
        }

        public override void Activate() { }

        public override void BindToBoard(SEventManager boardEventManager, IBoardNeuronsController controller, Hex position) {
            base.BindToBoard(boardEventManager, controller, position);
            BoardEventManager.Register(ExternalBoardEvents.OnPlaceElement, Decay);
        }

        private void Decay(EventArgs args) {
            _turnsToDeath--;
            if (_turnsToDeath > 0) {
                return;
            }
            BoardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, Decay);
            Controller.RemoveNeuron(Position);
        }
    }
}