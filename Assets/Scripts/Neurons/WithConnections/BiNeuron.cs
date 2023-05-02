namespace Neurons.WithConnections {
    public class BiNeuron : Neuron{
        public override NeuronType Type => NeuronType.Bi;
        
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}