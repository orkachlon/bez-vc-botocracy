namespace Neurons.WithNumLimit {
    public class Neuron1 : Neuron {
        public override ENeuronType Type => ENeuronType.One;
        
        public override int AllowedNeighbors() {
            return 1;
        }
    }
}