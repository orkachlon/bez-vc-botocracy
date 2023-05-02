namespace Neurons.WithConnections {
    public class CornerNeuron : Neuron {
        public override NeuronType Type => NeuronType.Corner;
          
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}