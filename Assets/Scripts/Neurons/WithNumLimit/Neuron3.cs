namespace Neurons.WithNumLimit {
    public class Neuron3 : Neuron {
        public override ENeuronType Type => ENeuronType.Three;
        
        public override int AllowedNeighbors() {
            return 3;
        }
    }
}