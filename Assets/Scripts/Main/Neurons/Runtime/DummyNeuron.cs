using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {
        public DummyNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy)) { }

        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) { }
    }
}