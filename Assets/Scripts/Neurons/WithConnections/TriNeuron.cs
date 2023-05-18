namespace Neurons.WithConnections {
    public class TriNeuron : Neuron {
        
        public override ENeuronType Type => ENeuronType.Tri;
           
        public override int AllowedNeighbors() {
            return 3;
        }
    }
}