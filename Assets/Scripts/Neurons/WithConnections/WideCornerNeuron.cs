namespace Neurons.WithConnections {
    public class WideCornerNeuron : Neuron {
        public override NeuronType Type => NeuronType.WideCorner;
           
        public override int AllowedNeighbors() {
            return 2;
        }
    }
}