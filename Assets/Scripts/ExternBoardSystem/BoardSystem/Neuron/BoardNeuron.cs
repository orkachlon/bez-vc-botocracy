using ExternBoardSystem.BoardSystem.Position;

namespace ExternBoardSystem.BoardSystem.Neuron {
    public class BoardNeuron : BoardElement {
        public BoardNeuron(NeuronData dataProvider) : base(dataProvider) {
        }
        
        public NeuronData ElementData => DataProvider as NeuronData;
    }
}