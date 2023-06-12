using Core.EventSystem;
using Core.Utils;
using Main.MyHexBoardSystem.BoardElements;
using Main.Neurons.Data;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.Neurons {
    public class Neuron : BoardNeuron {
        public Neuron(SNeuronDataBase dataProvider) : base(dataProvider) { }
        
        public Neuron(BoardNeuron boardNeuron) : base(boardNeuron.DataProvider) { }

        public ENeuronUIState UIState { get; set; }
        
        
        public override void Activate(SEventManager boardEventManager, IBoardNeuronsController controller, Vector3Int cell) {
            MLogger.LogEditor("Tried to activate UI neuron!!!");
            return;
        }
    }
    
    public enum ENeuronUIState {
        First = 0,
        Second = 1,
        Third = 2,
        Stack = 3
    }
}