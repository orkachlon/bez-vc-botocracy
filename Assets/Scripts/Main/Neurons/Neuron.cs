using Main.MyHexBoardSystem.BoardElements.Neuron;

namespace Main.Neurons {
    public class Neuron : BoardNeuron {
        public Neuron(SNeuronData dataProvider) : base(dataProvider) { }
        
        public Neuron(BoardNeuron boardNeuron) : base(boardNeuron.DataProvider) { }

        public ENeuronUIState UIState { get; set; }
    }


    public enum ENeuronUIState {
        First = 0,
        Second = 1,
        Third = 2,
        Stack = 3
    }
}