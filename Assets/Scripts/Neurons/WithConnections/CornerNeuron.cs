namespace Neurons.WithConnections {
    public class CornerNeuron : Neuron {
        public override ENeuronType Type => ENeuronType.Corner;
          
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}