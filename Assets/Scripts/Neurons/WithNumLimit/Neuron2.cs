namespace Neurons.WithNumLimit {
    public class Neuron2 : Neuron {
        public override ENeuronType Type => ENeuronType.Two;
        
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}