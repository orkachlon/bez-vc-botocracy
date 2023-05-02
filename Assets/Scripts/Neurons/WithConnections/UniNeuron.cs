namespace Neurons.WithConnections {
    public class UniNeuron : Neuron {
        public override NeuronType Type => NeuronType.Uni;
         
        public override int AllowedNeighbors() {
            return 1;
        }
    }
}
