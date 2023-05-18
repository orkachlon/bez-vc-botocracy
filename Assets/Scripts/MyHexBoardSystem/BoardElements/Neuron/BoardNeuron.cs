using ExternBoardSystem.BoardElements;

namespace MyHexBoardSystem.BoardElements.Neuron {
    public class BoardNeuron : BoardElement {
        public BoardNeuron(SNeuronData dataProvider) : base(dataProvider) {
        }
        
        public SNeuronData ElementData => DataProvider as SNeuronData;
    }
}