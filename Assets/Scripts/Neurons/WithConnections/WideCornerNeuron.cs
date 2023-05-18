namespace Neurons.WithConnections {
    public class WideCornerNeuron : Neuron {
        public override ENeuronType Type => ENeuronType.WideCorner;
           
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}