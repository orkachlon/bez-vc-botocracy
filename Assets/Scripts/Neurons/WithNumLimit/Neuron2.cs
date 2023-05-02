namespace Neurons.WithNumLimit {
    public class Neuron2 : Neuron {
        public override NeuronType Type => NeuronType.Two;
        
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}