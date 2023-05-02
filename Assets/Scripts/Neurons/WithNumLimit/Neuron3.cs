namespace Neurons.WithNumLimit {
    public class Neuron3 : Neuron {
        public override NeuronType Type => NeuronType.Three;
        
        public override int AllowedNeighbors() {
            return 3;
        }
    }
}