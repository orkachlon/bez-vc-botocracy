namespace Neurons.WithConnections {
    public class BiNeuron : Neuron{
        public override ENeuronType Type => ENeuronType.Bi;
        
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}