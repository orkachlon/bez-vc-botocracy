using ExternBoardSystem.BoardElements;

namespace MyHexBoardSystem.BoardElements.Neuron {
    public class BoardNeuron : BoardElement {
        public BoardNeuron(SNeuronData dataProvider) : base(dataProvider) {
        }

        public BoardNeuron(Neurons.MNeuron neuron) : this(MNeuronTypeToBoardData.GetNeuronData(neuron.Type)) {
        }
        
        public SNeuronData ElementData => DataProvider as SNeuronData;
    }
}