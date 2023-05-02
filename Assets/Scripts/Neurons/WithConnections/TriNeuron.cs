namespace Neurons.WithConnections {
    public class TriNeuron : Neuron {
        
        public override NeuronType Type => NeuronType.Tri;
           
        public override int AllowedNeighbors() {
            return 3;
        }
    }
}