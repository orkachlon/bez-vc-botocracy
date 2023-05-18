namespace Neurons.WithConnections {
    public class UniNeuron : Neuron {
        public override ENeuronType Type => ENeuronType.Uni;
         
        public override int AllowedNeighbors() {
            return 1;
        }
    }
}
