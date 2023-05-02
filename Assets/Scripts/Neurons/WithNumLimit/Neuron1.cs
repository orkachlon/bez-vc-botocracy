namespace Neurons.WithNumLimit {
    public class Neuron1 : Neuron {
        public override NeuronType Type => NeuronType.One;
        
        public override int AllowedNeighbors() {
            return 1;
        }
    }
}