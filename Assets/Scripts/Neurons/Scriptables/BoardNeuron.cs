using ExternBoardSystem.BoardSystem.Position;

namespace Neurons.Scriptables {
    public class BoardNeuron : BoardElement {
        public BoardNeuron(IElementDataProvider elementDataProvider) : base(elementDataProvider) { }
        public NeuronData ElementData => DataProvider as NeuronData;
    }
}