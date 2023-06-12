using Core.EventSystem;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {
        public InvulnerableNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable)) { }

        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) { }
    }
}