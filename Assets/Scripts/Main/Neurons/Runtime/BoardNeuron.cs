using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons.Data;
using UnityEngine;

namespace Main.Neurons.Runtime {
    public abstract class BoardNeuron : BoardElement {
        protected BoardNeuron(SNeuronDataBase dataProvider) : base(dataProvider) {
        }

        public new SNeuronDataBase DataProvider => base.DataProvider as SNeuronDataBase;

        public abstract void Activate(SEventManager boardEventManager, IBoardNeuronsController controller,
            Vector3Int cell);
    }
}